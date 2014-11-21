using System.Collections.Generic;

namespace uMigrations
{
    public interface IMigration
    {
        List<IMigrationStep> MigrationSteps { get; }
    }

    public class FluentMigrationStepDefinition
    {
        public FluentMigrationStepDefinition()
            : this(new List<IMigrationAction>())
        {}

        public FluentMigrationStepDefinition(List<IMigrationAction> actions)
        {
            Actions = actions;
        }

        public virtual List<IMigrationAction> Actions { get; private set; }

        public virtual IEnumerable<IMigrationAction> GetActions()
        {
            return Actions;
        }
    }

    public static class FluentExtensions
    {
        public class MovePropertyClause
        {
            public FluentMigrationStepDefinition Migration { get; set; }
            public string PropertyAlias { get; set; }

            public MovePropertyClause(FluentMigrationStepDefinition migration, string propertyAlias)
            {
                Migration = migration;
                PropertyAlias = propertyAlias;
            }
        }
        public static MovePropertyClause MoveProperty(this FluentMigrationStepDefinition migration, string propertyAlias)
        {
            return new MovePropertyClause(migration, propertyAlias);
        }
    }

    //public interface IMigrationActionProvider
    //{
    //    IMigrationAction        
    //}
}