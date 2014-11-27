using System;
using System.Collections.Generic;
using System.Linq;

namespace uMigrations
{
    public abstract class MigrationStepBase : IMigrationStep
    {
        private readonly Lazy<IEnumerable<IMigrationAction>> _applyActions;

        protected MigrationStepBase()
        {
            _applyActions = new Lazy<IEnumerable<IMigrationAction>>(() =>
            {
                var migrationDefinition = new FluentMigrationStepDefinition();
                Apply(migrationDefinition);
                return migrationDefinition.GetActions();
            });
        }

        public virtual string Name
        {
            get { return GetType().FullName; }
        }

        public virtual bool IsApplicable
        {
            get
            {
                var result = ApplyActions.All(x => x.IsApplicable);
                return result;
            }
        }

        public virtual IEnumerable<IMigrationAction> ApplyActions
        {
            get { return _applyActions.Value; }
        }

        protected abstract void Apply(FluentMigrationStepDefinition migration);
    }
}