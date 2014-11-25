using System;
using System.Collections.Generic;
using log4net;
using Umbraco.Core.Models;

namespace uMigrations
{
    public abstract class MovePropertyBase<TParameters> : IMigrationAction
    {
        protected MovePropertyBase(TParameters parameters,
            MigrationsSettings migrationSettings,
            IContentMigrationService contentMigrationService,
            ILog log)
        {
            Parameters = parameters;
            MigrationSettings = migrationSettings;
            ContentMigrationService = contentMigrationService;
            Log = log;
        }

        public virtual List<Exception> Validate()
        {
            return DoValidate(Parameters);
        }

        public virtual void Run()
        {
            DoRun(Parameters);
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

        protected IContentMigrationService ContentMigrationService { get; private set; }
        protected MigrationsSettings MigrationSettings { get; private set; }
        protected ILog Log { get; private set; }

        protected virtual string GetTempPropertyName(string propertyAlias)
        {
            var result = propertyAlias + "_" + MigrationSettings.MigrationRuntimeId;
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

        protected void UpdateContent(string setPropertyAlias, string getPropertyAlias, bool mandatory, object defaultValue)
        {
            // ContentMigrationService.RepublishAllContent();
            // todo: use parallel execution here
            foreach (var content in GetContentToUpdate())
            {
                object value = null;
                if (content.HasProperty(getPropertyAlias))
                {
                    value = content.GetValue(getPropertyAlias);
                }
                
                if (value == null /* && mandatory */ && defaultValue != null)
                {
                    value = defaultValue;
                }

                content.SetValue(setPropertyAlias, value);

                ContentMigrationService.UpdateContent(content);
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

            if (tabName != null)
            {
                // todo ? do we need to create a new property group ?
                contentType.AddPropertyType(newProperty, tabName);
            }
            else
            {
                contentType.AddPropertyType(newProperty);
            }

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

        protected bool ValidateContentIsOfType(List<Exception> migrationProblems, IContent content, string contentType)
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