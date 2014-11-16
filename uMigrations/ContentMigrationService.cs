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

        public ContentMigrationService(IContentTypeService contentTypeService, IContentService contentService)
        {
            ContentTypeService = contentTypeService;
            ContentService = contentService;
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
    }
}