using System.Collections.Generic;
using System.Linq;

namespace uMigrations.Tests.features.MovePropertyUp
{
    public class TestMigrationStep : IMigrationStep
    {
        private readonly IEnumerable<IMigrationAction> _actions;

        public TestMigrationStep(string name, IEnumerable<IMigrationAction> actions)
        {
            _actions = actions;
            Name = name;
        }

        public virtual string Name { get; private set; }
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
            get { return _actions; }
        }
    }
}