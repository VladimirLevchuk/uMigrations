using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using log4net;
using Umbraco.Core.Models;

namespace uMigrations
{
    public class MovePropertyUp : MovePropertyBase<MovePropertyUpParameters>
    {
        public override string ActionName
        {
            get { return "Move Property Up"; }
        }

        public MovePropertyUp(MovePropertyUpParameters parameters, 
            MigrationsSettings migrationSettings,
            IContentMigrationService contentMigrationService, 
            ILog log) : base(parameters, migrationSettings, contentMigrationService, log)
        {}

        public MovePropertyUp(MovePropertyUpParameters parameters, MigrationContext context)
            : this(parameters, context.MigrationSettings, context.ContentMigrationService, context.LogFactoryMethod(typeof(MovePropertyUp)))
        {}
        
        protected override List<IContent> GetContentToUpdate()
        {
            var contentType = ContentMigrationService.GetContentType(Parameters.DestinationTypeAlias);
            var result = ContentMigrationService.GetContentOfTypeOrDerived(contentType);
            return result;
        }
        
        protected override List<Exception> DoValidate(MovePropertyUpParameters parameters)
        {
            IEnumerable<string> sourceTypes = parameters.SourceTypes;
            string destinationTypeAlias = parameters.DestinationTypeAlias;
            string propertyAlias = parameters.PropertyAlias;
            bool mandatory = parameters.Mandatory;
            object defaultValue = parameters.DefaultValue;

            // check content supports both source and destination type
            var migrationProblems = new List<Exception>();

            var destinationContentType = ContentMigrationService.GetContentType(destinationTypeAlias);
            if (destinationContentType == null)
            {
                throw new Exception(string.Format("Destination Content type '{0}' not found. ", destinationTypeAlias));
            }

            int? propertyDataType = null;

            // todo: measure performance and cache property types if needed
            foreach (var sourceType in sourceTypes)
            {

                var sourceContentType = ValidateAndGetContentType(migrationProblems, sourceType);
                if (sourceContentType == null)
                {
                    continue;
                }

                var property = ValidateAndGetProperty(migrationProblems, sourceContentType, propertyAlias);

                if (property != null)
                {
                    if (propertyDataType.HasValue)
                    {
                        if (property.DataTypeDefinitionId != propertyDataType.Value)
                        {
                            var message = string.Format("Property '{0}' of type '{1}' data type id is '{2}' but '{3}' is expected. ",
                                propertyAlias, sourceType, property.DataTypeDefinitionId, propertyDataType);
                            migrationProblems.Add(new InvalidOperationException(message));
                        }
                    }
                    else
                    {
                        propertyDataType = property.DataTypeDefinitionId;
                    }
                }
            }

            return migrationProblems;
        }

        protected override void DoRun(MovePropertyUpParameters parameters)
        {
            IEnumerable<string> sourceTypes = parameters.SourceTypes;
            string destinationTypeAlias = parameters.DestinationTypeAlias;
            string propertyAlias = parameters.PropertyAlias;
            bool mandatory = parameters.Mandatory;
            object defaultValue = parameters.DefaultValue;
            var tabName = parameters.TabName;

            var sourceContentTypes = sourceTypes.Select(ContentMigrationService.GetContentType).ToList();
            var destinationContentType = ContentMigrationService.GetContentType(destinationTypeAlias);

            var oldProperties = sourceContentTypes.Select(x => ContentMigrationService.GetPropetyType(x, propertyAlias)).ToList();

            var propertyToCreateaNewOneFrom = oldProperties.FirstOrDefault();
            if (propertyToCreateaNewOneFrom == null)
            {
                throw new InvalidOperationException("No properties to move. ");
            }

            var newPropertyTempName = GetTempName(propertyAlias);
            var newProperty = CreatePropertyType(destinationContentType,
                tabName, newPropertyTempName, propertyToCreateaNewOneFrom);

            UpdateContent(newPropertyTempName, propertyAlias, mandatory, defaultValue);
            
            if (mandatory)
            {
                newProperty.Mandatory = true;
            }

            RenameProperty(destinationContentType, newProperty, propertyAlias);

            RemoveProperties(sourceContentTypes, propertyAlias);

            ProcessTab(destinationTypeAlias, propertyAlias, tabName);
        }

        private void ProcessTab(string contentTypeAlias, string propertyAlias, string tabName)
        {
            var contentType = ContentMigrationService.GetContentType(contentTypeAlias);
            var property = ContentMigrationService.GetPropetyType(contentType, propertyAlias);
            ProcessTab(contentType, property, tabName);
        }

        private void ProcessTab(IContentType contentType, PropertyType property, string tabName)
        {
            // if tab exists
            if (contentType.PropertyGroups.Contains(tabName))
            {
                contentType.MovePropertyType(property.Alias, tabName);
            }
            // if tab is not exists yet
            else
            {
                var derivedTypes = ContentMigrationService.GetDerivedTypes(contentType);
                var derivedTypesToProcess = derivedTypes.Where(x => x.PropertyGroups.Contains(tabName)).ToList();

                // if tab exists in any of derived types
                if (derivedTypesToProcess.Count > 0)
                {
                    // create a new tab with temp name
                    var tempTabName = GetTempName(tabName);
                    contentType.AddPropertyGroup(tempTabName);
                    ContentMigrationService.UpdateContentType(contentType);

                    // foreach derived type:
                    foreach (var derivedType in derivedTypesToProcess)
                    {
                        // move all properties from existing tab to a newly created
                        var tabProperties = derivedType.PropertyGroups[tabName].PropertyTypes.Select(x => x.Alias);
                        foreach (var tabProperty in tabProperties)
                        {
                            var moved = derivedType.MovePropertyType(tabProperty, tempTabName);
                            Debug.Assert(moved, "Add logging and exception");
                            ContentMigrationService.UpdateContentType(derivedType);
                        }
                    }

                    // foreach derived type: 
                    foreach (var derivedType in derivedTypesToProcess)
                    {
                        // remove all existing tabs (with no properties)
                        derivedType.RemovePropertyGroup(tabName);
                        ContentMigrationService.UpdateContentType(derivedType);
                    }

                    // rename tab to target name                
                    var tab = contentType.PropertyGroups[tempTabName];
                    tab.Name = tabName;
                    ContentMigrationService.UpdateContentType(contentType);
                }
                // otherwise we need to create a new tab and move property there
                else
                {
                    var propertyGroupCreated = contentType.AddPropertyGroup(tabName);
                    var propertyMoved = contentType.MovePropertyType(property.Alias, tabName);
                    ContentMigrationService.UpdateContentType(contentType);

                    if (!propertyGroupCreated || !propertyMoved)
                    {
                        var message = string.Format("Unable to move property '{0}' of content type '{1}' to tab '{2}'. Tab Created: {3}, Property Moved: {4}",
                            property.Alias, contentType.Alias, tabName, propertyGroupCreated, propertyMoved);

                        Log.Warn(message);

                        throw new InvalidOperationException(message);
                    }
                }
            }
        }
    }
}