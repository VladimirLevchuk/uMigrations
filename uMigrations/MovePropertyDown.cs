using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Umbraco.Core.Models;

namespace uMigrations
{
    public class MovePropertyDown : MovePropertyBase<MovePropertyDownParameters>
    {
        public override string ActionName
        {
            get { return "Move Property Down"; }
        }

        public MovePropertyDown(
            MovePropertyDownParameters parameters,
            IMigrationSettings migrationSettings,
            IContentMigrationService contentMigrationService, 
            ILog log) : base(parameters, migrationSettings, contentMigrationService, log)
        {}

        public MovePropertyDown(MovePropertyDownParameters parameters, MigrationContext context)
            : this(parameters, context.MigrationSettings, context.ContentMigrationService, context.LogFactoryMethod(typeof(MovePropertyDown)))
        {}

        protected override List<IContent> GetContentToUpdate()
        {
            return ContentMigrationService.GetContentOfTypes(Parameters.DestinationTypes).ToList();
        }

        protected override List<Exception> DoValidate(MovePropertyDownParameters parameters)
        {
            // check source property exists
            var sourceType = parameters.SourceTypeAlias;
            var propertyAlias = parameters.PropertyAlias;

            var migrationProblems = new List<Exception>();

            var sourceContentType = ValidateAndGetContentType(migrationProblems, sourceType);
            if (sourceContentType != null)
            {
                var property = ValidateAndGetProperty(migrationProblems, sourceContentType, propertyAlias);
            }

            return migrationProblems;
        }

        protected override void DoRun(MovePropertyDownParameters parameters)
        {
            IEnumerable<string> destinationTypes = parameters.DestinationTypes;
            string sourceTypeAlias = parameters.SourceTypeAlias;
            string propertyAlias = parameters.PropertyAlias;
            bool mandatory = parameters.Mandatory;
            object defaultValue = parameters.DefaultValue;
            var tabName = parameters.TabName;

            var destinationContentTypes = destinationTypes.Select(ContentMigrationService.GetContentType).ToList();
            var sourceContentType = ContentMigrationService.GetContentType(sourceTypeAlias);
            var sourceProperty = ContentMigrationService.GetPropetyType(sourceContentType, propertyAlias);

            var propertyToCreateaNewOneFrom = sourceProperty;
            var newPropertyTempName = GetTempName(propertyAlias);

            foreach (var destinationContentType in destinationContentTypes)
            {
                CreatePropertyType(destinationContentType, tabName, newPropertyTempName, propertyToCreateaNewOneFrom);
            }

            // ContentMigrationService.RepublishAllContent();
            // ContentMigrationService.RepublishAllContent();
            UpdateContent(newPropertyTempName, propertyAlias, mandatory, defaultValue);

            RemoveProperty(sourceContentType, propertyAlias);
            // ContentMigrationService.RepublishAllContent();

            destinationContentTypes = destinationTypes.Select(ContentMigrationService.GetContentType).ToList();
            
            foreach (var destinationContentType in destinationContentTypes)
            {
                var newProperty = ContentMigrationService.GetPropetyType(destinationContentType, newPropertyTempName);
                if (mandatory)
                {
                    newProperty.Mandatory = true;
                }

                RenameProperty(destinationContentType, newProperty, propertyAlias);
            }

            // ContentMigrationService.RepublishAllContent();
        }
    }
}