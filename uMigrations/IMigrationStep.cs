using System.Collections.Generic;

namespace uMigrations
{
    public interface IMigrationStep
    {
        string Name { get; }

        bool IsApplicable { get; }

        IEnumerable<IMigrationAction> ApplyActions { get; }
    }
}