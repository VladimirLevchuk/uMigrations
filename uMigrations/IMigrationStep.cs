using System.Collections.Generic;

namespace uMigrations
{
    public interface IMigrationStep
    {
        string Name { get; }

        IEnumerable<IMigrationAction> ApplyActions { get; }

        IEnumerable<IMigrationAction> RemoveActions { get; }
    }
}