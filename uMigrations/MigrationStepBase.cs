using System;
using System.Collections.Generic;

namespace uMigrations
{
    public abstract class MigrationStepBase : IMigrationStep
    {
        private readonly Lazy<IEnumerable<IMigrationAction>> _upgradeActions;
        private readonly Lazy<IEnumerable<IMigrationAction>> _downgradeActions;

        protected MigrationStepBase()
        {
            _upgradeActions = new Lazy<IEnumerable<IMigrationAction>>(() =>
            {
                var migrationDefinition = new FluentMigrationStepDefinition();
                Apply(migrationDefinition);
                return migrationDefinition.GetActions();
            });

            _downgradeActions = new Lazy<IEnumerable<IMigrationAction>>(() =>
            {
                var migrationDefinition = new FluentMigrationStepDefinition();
                Remove(migrationDefinition);
                return migrationDefinition.GetActions();
            });
        }

        public virtual string Name
        {
            get { return GetType().FullName; }
        }

        public virtual IEnumerable<IMigrationAction> ApplyActions
        {
            get { return _upgradeActions.Value; }
        }

        public virtual IEnumerable<IMigrationAction> RemoveActions
        {
            get { return _downgradeActions.Value; }
        }

        protected abstract void Apply(FluentMigrationStepDefinition migration);

        protected abstract void Remove(FluentMigrationStepDefinition migration);
    }
}