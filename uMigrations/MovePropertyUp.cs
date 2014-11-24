using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Umbraco.Core.Models;

namespace uMigrations
{
    public class MovePropertyUp : MovePropertyBase
    {
        public MovePropertyUp(MovePropertyUpParameters parameters, 
            MigrationsSettings migrationSettings,
            IContentMigrationService contentMigrationService, 
            ILog log)
        {
            Parameters = parameters;
            MigrationSettings = migrationSettings;
            Log = log;
            ContentMigrationService = contentMigrationService;
            _contentToUpdate = new Lazy<List<IContent>>(GetContentToUpdate);
        }

        public MovePropertyUp(MovePropertyUpParameters parameters, MigrationContext context)
            : this(parameters, context.MigrationSettings, context.ContentMigrationService, context.LogFactoryMethod(typeof(MovePropertyUp)))
        {}

        protected IContentMigrationService ContentMigrationService { get; private set; }
        public MovePropertyUpParameters Parameters { get; private set; }
        protected MigrationsSettings MigrationSettings { get; private set; }
        protected ILog Log { get; private set; }

        private readonly Lazy<List<IContent>> _contentToUpdate;
        
        protected virtual List<IContent> GetContentToUpdate()
        {
            return ContentMigrationService.GetContentOfTypes(Parameters.SourceTypes).ToList();
        }

        protected virtual List<IContent> ContentToUpdate
        {
            get
            {
                return GetContentToUpdate();
                //return _contentToUpdate.Value;
            }
        }

        public override string ToString()
        {
            return string.Format("Migration Action 'Move Property Up' with parameters {0}", Parameters);
        }

        protected virtual List<Exception> DoValidate(MovePropertyUpParameters parameters)
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
                var sourceContentType = ContentMigrationService.GetContentType(sourceType);
                if (sourceContentType == null)
                {
                    migrationProblems.Add(new Exception(string.Format("Content type '{0}' not found. ", sourceType)));
                    continue;
                }

                var property = ContentMigrationService.GetPropetyType(sourceContentType, propertyAlias);

                if (property == null)
                {
                    var message = string.Format("Property '{0}' is not found in type '{1}' ",
                        propertyAlias, sourceType);
                    migrationProblems.Add(new InvalidOperationException(message));
                }
                else
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

            foreach (var content in ContentToUpdate)
            {
                if (!ContentMigrationService.IsContentOfType(content, destinationTypeAlias))
                {
                    string message = string.Format("Content item #{0} of type '{1}' is not of type '{2}'", content.Id,
                        content.ContentType.Alias, destinationTypeAlias);
                    migrationProblems.Add(new InvalidOperationException(message));
                }

                if (mandatory && defaultValue == null)
                {
                    string message = string.Format("No value for mandatory field '{0}' on content item id #{1}", 
                        propertyAlias, content.Id);

                    if (content.GetValue(propertyAlias) == null)
                    {
                        migrationProblems.Add(new InvalidOperationException(message));
                    }
                }
            }

            return migrationProblems;
        }

        public override List<Exception> Validate()
        {
            return DoValidate(Parameters);
        }

        protected virtual void DoRun(MovePropertyUpParameters parameters)
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
            var newProperty = CreateDestinationPropertyType(destinationContentType,
                tabName, newPropertyTempName, propertyToCreateaNewOneFrom);

            ContentMigrationService.RepublishAllContent();

            UpdateContent(newPropertyTempName, propertyAlias, mandatory, defaultValue);
            
            if (mandatory)
            {
                newProperty.Mandatory = true;
            }

            newProperty.Alias = propertyAlias;
            ContentMigrationService.UpdateContentType(destinationContentType);

            RemoveSourceProperties(sourceContentTypes, propertyAlias);
        }

        private void RemoveSourceProperties(List<IContentType> sourceContentTypes, string oldPropertyTempAlias)
        {
            sourceContentTypes.ForEach(x =>
            {
                x.RemovePropertyType(oldPropertyTempAlias);
                ContentMigrationService.UpdateContentType(x);
            });
        }

        private void UpdateContent(string setPropertyAlias, string getPropertyAlias, bool mandatory, object defaultValue)
        {
            // todo: use parallel execution here
            foreach (var content in ContentToUpdate)
            {
                var value = content.GetValue(getPropertyAlias);

                if (value == null && mandatory && defaultValue != null)
                {
                    value = defaultValue;
                }

                content.SetValue(setPropertyAlias, value);

                ContentMigrationService.UpdateContent(content);
            }
        }

        //private string RenameSourceProperties(IEnumerable<PropertyType> oldProperties, string propertyAlias)
        //{
        //    string oldPropertyTempAlias = GetTempPropertyName(propertyAlias);

        //    foreach (var oldProperty in oldProperties)
        //    {
        //        ContentMigrationService.RenameProperty(oldProperty, oldPropertyTempAlias);
        //        // ContentMigrationService.UpdateContentType();
        //    }
            
        //    return oldPropertyTempAlias;
        //}

        protected virtual string GetTempPropertyName(string propertyAlias)
        {
            var result = propertyAlias + "_" + MigrationSettings.MigrationRuntimeId;
            return result;
        }

        private PropertyType CreateDestinationPropertyType(
            IContentType destinationContentType,
            string tabName,
            string propertyAlias, 
            PropertyType propertyToCreateANewOneFrom)
        {
            var newProperty = ContentMigrationService.CopyPropertyType(propertyAlias, propertyToCreateANewOneFrom);
            
            newProperty.Mandatory = false;

            if (tabName != null)
            {
                // todo ? do we need to create a new property group ?
                destinationContentType.AddPropertyType(newProperty, tabName);
            }
            else
            {
                destinationContentType.AddPropertyType(newProperty);
            }

            ContentMigrationService.UpdateContentType(destinationContentType);

            return newProperty;
        }

        public override void Run()
        {
            DoRun(Parameters);
        }
    }
}