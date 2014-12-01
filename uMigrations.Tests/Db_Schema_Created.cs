using System.Linq;
using NUnit.Framework;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace uMigrations.Tests
{

    public class DbTable
    {
        public string Name { get; set; }
    }

    [SetUpFixture]
    public class AppStart
    {
        TestEnvironment _environment = new TestEnvironment();

        [SetUp]
        public void BeforeAll()
        {
            _environment.Startup();
        }

        [TearDown]
        public void AfterAll()
        {
            _environment.Shutdown();
        }
    }

    public class Db_Schema_Created
    {
        public Database Db { get { return ApplicationContext.Current.DatabaseContext.Database; } }

        [Test]
        public void When_Test_Started_Db_Schema_Created()
        {
            var tables = Db.Query<DbTable>("SELECT [name] FROM sys.tables").ToList();
            Assert.That(tables.Count > 0);
        }
    }
}