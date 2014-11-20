using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using uMigrations.Metadata;

namespace uMigrations
{
    public class ContentMigrationService : IContentMigrationService
    {
        protected IContentTypeService ContentTypeService { get; private set; }
        protected IContentService ContentService { get; private set; }
        protected MigrationsSettings MigrationsSettings { get; private set; }

        public ContentMigrationService(IContentTypeService contentTypeService, 
            IContentService contentService,
            MigrationsSettings migrationsSettings)
        {
            ContentTypeService = contentTypeService;
            ContentService = contentService;
            MigrationsSettings = migrationsSettings;
        }

        public virtual IEnumerable<IContent> GetContentOfType(string contentTypeAlias)
        {
            var contentType = GetContentType(contentTypeAlias);
            var result = ContentService.GetContentOfContentType(contentType.Id);
            return result;
        }

        public virtual bool IsContentOfType(IContent content, string contentTypeAlias)
        {
            var typeToCheck = GetContentType(contentTypeAlias);
            var id = typeToCheck.Id.ToString(CultureInfo.InvariantCulture);
            var ids = content.ContentType.Path.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var result = ids.Contains(id);
            return result;
        }

        public virtual IContentType GetContentType(string contentTypeAlias)
        {
            return ContentTypeService.GetContentType(contentTypeAlias);
        }

        public void UpdateContent(IContent content)
        {
            if (content.Published)
            {
                ContentService.SaveAndPublishWithStatus(content, GetSystemUserId(), false);
            }
            else
            {
                ContentService.Save(content, GetSystemUserId(), false);
            }
        }

        protected virtual int GetSystemUserId()
        {
            return MigrationsSettings.SystemUserId;
        }

        public void UpdateContentTypes(params IContentType[] contentTypes)
        {
            foreach (var contentType in contentTypes)
            {
                ContentTypeService.Save(contentType);
            }
        }
    }
}