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
            MigrationsSettings migrationSettings,
            IContentMigrationService contentMigrationService, 
            ILog log) : base(parameters, migrationSettings, contentMigrationService, log)
        {}

        public MovePropertyDown(MovePropertyDownParameters parameters, MigrationContext context)
            : this(parameters, context.MigrationSettings, context.ContentMigrationService, context.LogFactoryMethod(typeof(MovePropertyDown)))
        {}

        protected override List<IContent> GetContentToUpdate()
        {
            return ContentMigrationService.GetContentOfType(Parameters.SourceTypeAlias).ToList();
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
            var newPropertyTempName = GetTempPropertyName(propertyAlias);

            foreach (var destinationContentType in destinationContentTypes)
            {
                CreatePropertyType(destinationContentType, tabName, newPropertyTempName, propertyToCreateaNewOneFrom);
            }

            ContentMigrationService.RepublishAllContent();
            ContentMigrationService.RepublishAllContent();
            UpdateContent(newPropertyTempName, propertyAlias, mandatory, defaultValue);

            RemoveProperty(sourceContentType, propertyAlias);
            ContentMigrationService.RepublishAllContent();

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

            ContentMigrationService.RepublishAllContent();
        }
    }

    public static class MovePropertyDownFluentExtensions
    {
        public class MovePropertyToChildTypesClause
        {
            public FluentExtensions.MovePropertyClause MovePropertyClause { get; set; }
            public List<string> DestinationContentTypes { get; set; }
            public object DefaultValue { get; set; }
            public bool IsDefaultSet { get; set; }
            public bool IsMandatory { get; set; }
            public string TabName { get; set; }

            public MovePropertyToChildTypesClause(FluentExtensions.MovePropertyClause movePropertyClause, List<string> destinationTypes)
            {
                MovePropertyClause = movePropertyClause;
                DestinationContentTypes = destinationTypes;
            }

            public MovePropertyToChildTypesClause Mandatory()
            {
                IsMandatory = true;
                return this;
            }

            public MovePropertyToChildTypesClause WithDefault(object value)
            {
                DefaultValue = value;
                IsDefaultSet = true;
                return this;
            }

            public MovePropertyToChildTypesClause ToTab(string tabName)
            {
                TabName = tabName;
                return this;
            }

            public void FromType(string sourceType)
            {
                var parameters = new MovePropertyDownParameters(sourceType,
                    DestinationContentTypes,
                    MovePropertyClause.PropertyAlias,
                    TabName,
                    IsMandatory,
                    DefaultValue);

                var action = new MovePropertyDown(parameters, MigrationContext.Current);
                MovePropertyClause.Migration.Actions.Add(action);
            }
        }

        public static MovePropertyToChildTypesClause ToDerivedTypes(this FluentExtensions.MovePropertyClause movePropertyClause,
            string destinationType, params string[] destinationTypes)
        {
            var types = new List<string>();
            types.Add(destinationType);
            types.AddRange(destinationTypes);
            
            return new MovePropertyToChildTypesClause(movePropertyClause, types);
        }
    }
}