using System;
using Umbraco.Core;
using uMigrations.Persistance;

namespace uMigrations.MvcTest
{
    public class Migrations
    {
        public static void Run()
        {
            MigrationContext.Current.Runner.Apply(new CustomMigration(new TestMigrationStep()));
        }
    }

    public class TestMigrationStep : MigrationStepBase
    {
        protected override void Apply(FluentMigrationStepDefinition migration)
        {
            migration.MoveProperty("level2Prop2").ToBaseType("Level1").ToTab("TestTab").FromTypes("Level2");
            migration.MoveProperty("moveToBase").ToBaseType("Level11").ToTab("TestTab").FromTypes("Level21", "Level22");
            migration.MoveProperty("moveToBase2").ToBaseType("Level11").ToTab("TestTab").Mandatory().WithDefault("default").FromTypes("Level21", "Level22");
        }

        //protected override void Apply(FluentMigrationStepDefinition migration)
        //{
        //    migration.MoveProperty("level2Prop2").ToDerivedTypes("Level2").FromType("Level1");
        //    migration.MoveProperty("moveToBase").ToDerivedTypes("Level21", "Level22").FromType("Level11");
        //    migration.MoveProperty("moveToBase2").ToDerivedTypes("Level21", "Level22").Mandatory().FromType("Level11");
        //}
    }
}