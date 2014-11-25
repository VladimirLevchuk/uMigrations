using System;
using System.Collections.Generic;
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
            return ContentMigrationService.GetContentOfType(Parameters.DestinationTypeAlias).ToList();
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

            //foreach (var content in GetContentToUpdate())
            //{
            //    ValidateContentIsOfType(migrationProblems, content, destinationTypeAlias);
            //    if (!ContentMigrationService.IsContentOfType(content, destinationTypeAlias))
            //    {
            //        string message = string.Format("Content item #{0} of type '{1}' is not of type '{2}'", content.Id,
            //            content.ContentType.Alias, destinationTypeAlias);
            //        migrationProblems.Add(new InvalidOperationException(message));
            //    }

            //    if (mandatory && defaultValue == null)
            //    {
            //        string message = string.Format("No value for mandatory field '{0}' on content item id #{1}", 
            //            propertyAlias, content.Id);

            //        if (content.GetValue(propertyAlias) == null)
            //        {
            //            migrationProblems.Add(new InvalidOperationException(message));
            //        }
            //    }
            //}

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

            var newPropertyTempName = GetTempPropertyName(propertyAlias);
            var newProperty = CreatePropertyType(destinationContentType,
                tabName, newPropertyTempName, propertyToCreateaNewOneFrom);

            // ContentMigrationService.RepublishAllContent();
            // ContentMigrationService.RepublishAllContent();
            UpdateContent(newPropertyTempName, propertyAlias, mandatory, defaultValue);
            
            if (mandatory)
            {
                newProperty.Mandatory = true;
            }

            RenameProperty(destinationContentType, newProperty, propertyAlias);

            RemoveProperties(sourceContentTypes, propertyAlias);
            // ContentMigrationService.RepublishAllContent();
        }

        //private PropertyType CreateDestinationPropertyType(
        //    IContentType destinationContentType,
        //    string tabName,
        //    string propertyAlias, 
        //    PropertyType propertyToCreateANewOneFrom)
        //{
        //    var newProperty = ContentMigrationService.CopyPropertyType(propertyAlias, propertyToCreateANewOneFrom);
            
        //    newProperty.Mandatory = false;

        //    if (tabName != null)
        //    {
        //        // todo ? do we need to create a new property group ?
        //        destinationContentType.AddPropertyType(newProperty, tabName);
        //    }
        //    else
        //    {
        //        destinationContentType.AddPropertyType(newProperty);
        //    }

        //    ContentMigrationService.UpdateContentType(destinationContentType);

        //    return newProperty;
        //}
    }
}