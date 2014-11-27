using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace uMigrations
{
    public class ContentMigrationService : IContentMigrationService
    {
        protected IContentTypeService ContentTypeService { get; private set; }
        protected IContentService ContentService { get; private set; }
        protected IDataTypeService DataTypeService { get; private set; }
        protected IMigrationSettings MigrationsSettings { get; private set; }

        public ContentMigrationService(
            IContentTypeService contentTypeService, 
            IContentService contentService,
            IDataTypeService dataTypeService,
            IMigrationSettings migrationsSettings)
        {
            ContentTypeService = contentTypeService;
            ContentService = contentService;
            DataTypeService = dataTypeService;
            MigrationsSettings = migrationsSettings;
        }

        public virtual List<IContent> GetContentOfType(string contentTypeAlias)
        {
            var contentType = GetContentType(contentTypeAlias);
            var result = ContentService.GetContentOfContentType(contentType.Id).ToList();
            return result;
        }

        public virtual List<IContent> GetContentOfType(IContentType contentType)
        {
            var result = ContentService.GetContentOfContentType(contentType.Id).ToList();
            return result;
        }

        public virtual List<IContent> GetContentOfTypeOrDerived(IContentType contentType)
        {
            var typeOrDerived = GetContentTypeOrDerivedTypes(contentType);
            var result = GetContentOfTypes(typeOrDerived).ToList();
            return result;
        }

        public virtual List<IContent> GetContentOfTypes(IEnumerable<string> contentTypeAliases)
        {
            return GetContentOfTypes(contentTypeAliases.Select(GetContentType));
        }

        public virtual List<IContent> GetContentOfTypes(IEnumerable<IContentType> contentTypes)
        {
            var query = contentTypes.SelectMany(GetContentOfType).DistinctBy(x => x.Id);
            var result = query.ToList();
            return result;
        }

        public bool IsContentOfType(IContent content, string contentTypeAlias)
        {
            var typeToCheck = GetContentType(contentTypeAlias);
            return IsContentOfType(content, typeToCheck);
        }

        public virtual bool IsContentTypeOfType(IContentType contentType, int ofTypeContentTypeId)
        {
            var id = ofTypeContentTypeId.ToString(CultureInfo.InvariantCulture);
            var ids = contentType.Path.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var result = ids.Contains(id);
            return result;
        }

        public virtual bool IsContentOfType(IContent content, IContentType contentType)
        {
            return IsContentOfType(content, contentType.Id);
        }

        public virtual bool IsContentOfType(IContent content, int contentTypeId)
        {
            return IsContentTypeOfType(content.ContentType, contentTypeId);
        }

        public virtual IContentType GetContentType(string contentTypeAlias)
        {
            return ContentTypeService.GetContentType(contentTypeAlias);
        }

        public virtual void UpdateContent(IContent content)
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

        public virtual void UpdateContentType(IContentType contentType)
        {
            ContentTypeService.Save(contentType);
        }

        public virtual void RepublishAllContent()
        {
            ContentService.RePublishAll();
        }

        public virtual List<IContentType> GetContentTypeOrDerivedTypes(IContentType contentType)
        {
            var descendants = ContentTypeService.GetAllContentTypes().Where(x => IsContentTypeOfType(x, contentType.Id));
            var result = descendants.ToList();
            return result;
        }

        public virtual List<IContentType> GetDerivedTypes(IContentType contentType)
        {
            var descendants = ContentTypeService.GetAllContentTypes().Where(x => x.Alias != contentType.Alias && IsContentTypeOfType(x, contentType.Id));
            var result = descendants.ToList();
            return result;
        }

        public virtual PropertyType CopyPropertyType(string propertyAlias, PropertyType propertyType)
        {
            var dtd = DataTypeService.GetDataTypeDefinitionById(propertyType.DataTypeDefinitionId);
            var result = new PropertyType(dtd)
            {
                Alias = propertyAlias,
                Description = propertyType.Description,
                Mandatory = propertyType.Mandatory,
                SortOrder = propertyType.SortOrder,
                ValidationRegExp = propertyType.ValidationRegExp,
                Name = propertyType.Name
            };

            return result;
        }

        public virtual void RenameProperty(PropertyType propertyType, string newAlias)
        {
            if (propertyType == null) throw new ArgumentNullException("propertyType");
            propertyType.Alias = newAlias;
        }

        public virtual PropertyType GetPropetyType(IContentType contentType, string alias)
        {
            var result =
                contentType.PropertyTypes.FirstOrDefault(
                    x => x.Alias.Equals(alias, StringComparison.InvariantCultureIgnoreCase));
            return result;
        }
    }
}