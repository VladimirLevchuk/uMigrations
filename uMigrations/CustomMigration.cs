using System.Collections.Generic;
using System.Linq;

namespace uMigrations
{
    public class CustomMigration : MigrationBase
    {
        public CustomMigration(List<IMigrationStep> migrationSteps) : base(migrationSteps)
        {}

        public CustomMigration(params IMigrationStep[] steps)
            : this((List<IMigrationStep>) steps.ToList())
        {}
    }
}