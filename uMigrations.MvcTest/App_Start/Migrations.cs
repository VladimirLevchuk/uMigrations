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
            using (MigrationContext.Current.TransactionProvider.BeginTransaction())
            {
                new TestMigration().Up();
            }
        }
    }

    public class TestMigration : IMigration
    {
        public virtual void Up()
        {
            // MigrationContext.Current.Api.MoveProperty("SecondLevelDT2", "FirstLevelDT", "property1");
        }

        public virtual void Down()
        {
           
        }
    }
}