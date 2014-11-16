using System.Collections.Generic;
using Umbraco.Core.Models;
using uMigrations.Metadata;

namespace uMigrations
{
    public interface IContentMigrationService
    {
        IEnumerable<IContent> GetContentOfType(string contentTypeAlias);

        bool IsContentOfType(IContent content, string contentTypeAlias);
        IContentType GetContentType(string contentTypeAlias);
    }
}