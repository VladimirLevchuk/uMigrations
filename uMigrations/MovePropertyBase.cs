using System;
using System.Collections.Generic;
using log4net;
using Umbraco.Core.Models;

namespace uMigrations
{
    public abstract class MovePropertyBase<TParameters> : IMigrationAction, INoValueAnalyzer
    {
        private readonly List<int> _updatedContent = new List<int>();
        private readonly Lazy<INoValueAnalyzer> _noValueAnalyzer;

        protected MovePropertyBase(TParameters parameters,
            IMigrationSettings migrationSettings,
            IContentMigrationService contentMigrationService,
            ILog log)
        {
            Parameters = parameters;
            MigrationSettings = migrationSettings;
            ContentMigrationService = contentMigrationService;
            Log = log;
            _noValueAnalyzer = new Lazy<INoValueAnalyzer>(GetNoValueAnalyzer);
        }

        public virtual List<Exception> Validate()
        {
            return DoValidate(Parameters);
        }

        public virtual void Run()
        {
            DoRun(Parameters);
        }

        public virtual bool IsApplicable
        {
            get
            {
                // todo: refactor not to call DoValidate twice
                var errors = DoValidate(Parameters);
                return errors.Count == 0;
            }
        }

        public TParameters Parameters { get; private set; }

        public abstract string ActionName { get; }

        public override string ToString()
        {
            return string.Format("Migration Action '{0}' with parameters {1}", ActionName, Parameters);
        }

        protected abstract List<Exception> DoValidate(TParameters parameters);
        protected abstract void DoRun(TParameters parameters);
        protected abstract List<IContent> GetContentToUpdate();

        protected virtual List<int> UpdatedContent
        {
            get { return _updatedContent; }
        }

        protected IContentMigrationService ContentMigrationService { get; private set; }
        protected IMigrationSettings MigrationSettings { get; private set; }
        protected ILog Log { get; private set; }

        protected virtual string GetTempName(string alias)
        {
            var result = alias + "_" + MigrationSettings.MigrationRuntimeId;
            return result;
        }

        protected void RemoveProperty(IContentType contentType, string propertyAlias)
        {
            contentType.RemovePropertyType(propertyAlias);
            ContentMigrationService.UpdateContentType(contentType);
        }

        protected void RemoveProperties(List<IContentType> sourceContentTypes, string propertyAlias)
        {
            sourceContentTypes.ForEach(x => RemoveProperty(x, propertyAlias));
        }

        protected void RenameProperty(IContentType contentType, string propertyAlias, string propertyNewAlias)
        {
            var property = ContentMigrationService.GetPropetyType(contentType, propertyAlias);
            RenameProperty(contentType, property, propertyNewAlias);
        }

        protected void RenameProperty(IContentType contentType, PropertyType property, string propertyAlias)
        {
           property.Alias = propertyAlias;
           ContentMigrationService.UpdateContentType(contentType);
        }

        public virtual bool NoValue(object value)
        {
            return _noValueAnalyzer.Value.NoValue(value);
        }

        private INoValueAnalyzer GetNoValueAnalyzer()
        {
            var provider = Parameters as INoValueAnalyzerProvider;
            var result = (provider != null ? provider.NoValueAnalyzer : null) ?? new DefaultNoValueAnalyzer();
            return result;
        }

        protected void UpdateContent(string setPropertyAlias, string getPropertyAlias, bool mandatory, object defaultValue)
        {
            var contentToUpdate = GetContentToUpdate();
            
            UpdatedContent.Clear();

            foreach (var content in contentToUpdate)
            {
                object value = null;
                if (content.HasProperty(getPropertyAlias))
                {
                    value = content.GetValue(getPropertyAlias);
                }
                
                if (defaultValue != null && NoValue(value))
                {
                    value = defaultValue;
                }

                content.SetValue(setPropertyAlias, value);

                ContentMigrationService.UpdateContent(content);

                // invalidate cmsPropeprtyData cache to fix strange errors
                ContentMigrationService.GetContentById(content.Id);

                UpdatedContent.Add(content.Id);
            }
        }

        protected PropertyType CreatePropertyType(
            IContentType contentType,
            string tabName,
            string propertyAlias,
            PropertyType propertyToCreateANewOneFrom)
        {
            var newProperty = ContentMigrationService.CopyPropertyType(propertyAlias, propertyToCreateANewOneFrom);

            newProperty.Mandatory = false;

            contentType.AddPropertyType(newProperty);

            ContentMigrationService.UpdateContentType(contentType);

            return newProperty;
        }

        protected IContentType ValidateAndGetContentType(List<Exception> migrationProblems, string contentTypeAlias)
        {
            var contentType = ContentMigrationService.GetContentType(contentTypeAlias);
            if (contentType == null)
            {
                migrationProblems.Add(new Exception(string.Format("Content type '{0}' not found. ", contentTypeAlias)));
            }

            return contentType;
        }

        protected PropertyType ValidateAndGetProperty(List<Exception> migrationProblems, IContentType contentType, string propertyAlias)
        {
            var property = ContentMigrationService.GetPropetyType(contentType, propertyAlias);
            if (property == null)
            {
                var message = string.Format("Property '{0}' is not found in type '{1}' ",
                    propertyAlias, contentType.Alias);
                migrationProblems.Add(new InvalidOperationException(message));
            }

            return property;
        }

        protected bool ValidateContentIsOfType(List<Exception> migrationProblems, IContent content, IContentType contentType)
        {
            if (!ContentMigrationService.IsContentOfType(content, contentType))
            {
                string message = string.Format("Content item #{0} of type '{1}' is not of type '{2}'", content.Id,
                    content.ContentType.Alias, contentType);
                migrationProblems.Add(new InvalidOperationException(message));

                return false;
            }

            return true;
        }
    }
}