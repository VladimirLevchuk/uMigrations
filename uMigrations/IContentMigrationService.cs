using System.Collections.Generic;
using Umbraco.Core.Models;

namespace uMigrations
{
    public interface IContentMigrationService
    {
        List<IContent> GetContentOfType(string contentTypeAlias);
        List<IContent> GetContentOfTypes(IEnumerable<string> contentTypeAliases);

        bool IsContentOfType(IContent content, string contentTypeAlias);
        IContentType GetContentType(string contentTypeAlias);
        void UpdateContent(IContent content);
        void UpdateContentType(IContentType contentTypes);

        PropertyType CopyPropertyType(string propertyAlias, PropertyType propertyType);
        void RenameProperty(PropertyType propertyType, string newAlias);
        PropertyType GetPropetyType(IContentType contentType, string alias);
        void RepublishAllContent();
    }
}