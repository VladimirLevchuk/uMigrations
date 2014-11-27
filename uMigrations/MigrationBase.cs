using System;
using System.Collections.Generic;

namespace uMigrations
{
    public abstract class MigrationBase : IMigration
    {
        protected MigrationBase(List<IMigrationStep> migrationSteps)
        {
            MigrationSteps = migrationSteps;
        }

        public virtual List<IMigrationStep> MigrationSteps { get; private set; }
    }
}