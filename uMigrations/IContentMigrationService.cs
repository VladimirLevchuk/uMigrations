using System.Collections.Generic;
using Umbraco.Core.Models;
using uMigrations.Metadata;

namespace uMigrations
{
    public interface IContentMigrationService
    {
        List<IContent> GetContentOfType(string contentTypeAlias);
        List<IContent> GetContentOfTypes(IEnumerable<string> contentTypeAliases);

        bool IsContentOfType(IContent content, string contentTypeAlias);
        IContentType GetContentType(string contentTypeAlias);
        void UpdateContent(IContent content);
        void UpdateContentTypes(params IContentType[] contentTypes);
        PropertyType CopyPropertyType(string propertyAlias, PropertyType propertyType);
        string RenamePropertyForDeletion(PropertyType propertyType, string propertyAlias);
        PropertyType GetPropetyType(IContentType contentType, string alias);
    }
}