using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;
using Umbraco.Core;
using Umbraco.Core.Persistence.Migrations;
using Umbraco.Core.Services;

namespace uMigrations.MvcTest
{
    public class Migrations
    {
        public static void Run()
        {
            if (MigrationContext.Current.MigrationSettings.SkipMigrations)
            {
                return;
            }

            using (var tran = MigrationContext.Current.TransactionProvider.BeginTransaction())
            {
                new TestMigration().Up();
                tran.Commit();
            }
        }
    }

    public class TestMigration : IMigration
    {
        public virtual void Up()
        {
            // MigrationContext.Current.Api.MoveProperty("SecondLevelDT2", "FirstLevelDT", "property1");
            MigrationContext.Current.Api.MovePropertyUp("Level2", "Level1", "level2Prop2");
        }

        public virtual void Down()
        {
            MigrationContext.Current.Api.MovePropertyUp("Level1", "Level2", "level2Prop2");
        }
    }
}