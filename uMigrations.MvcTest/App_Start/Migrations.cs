using System;

namespace uMigrations.MvcTest
{
    public class Migrations
    {
        public static void Run()
        {
            MigrationContext.Current.Runner.Upgrade(new CustomMigration(new TestMigrationStep()));
        }
    }

    public class TestMigrationStep : MigrationStepBase
    {
        protected override void Apply(FluentMigrationStepDefinition migration)
        {
            migration.MoveProperty("level2Prop2").ToBaseType("Level1").FromTypes("Level2");
        }

        protected override void Remove(FluentMigrationStepDefinition migration)
        {
            // migration.MoveProperty("level2Prop2").ToDerivedTypes("Level2").FromType("Level1");
            throw new NotImplementedException();
        }
    }
}