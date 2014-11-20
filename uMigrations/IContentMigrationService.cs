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
        void UpdateContent(IContent content);
        void UpdateContentTypes(params IContentType[] contentTypes);
    }

    public class MigrationsSettings
    {
        public virtual int SystemUserId
        {
            get { return 0; }
        }
    }
}