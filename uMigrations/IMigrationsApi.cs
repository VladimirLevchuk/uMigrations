using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uMigrations.Metadata;

namespace uMigrations
{
    public interface IMigrationsApi
    {
        void MoveProperty(string sourceTypeAlias, string destinationTypeAlias, string propertyAlias);
    }
}
