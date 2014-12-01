using NUnit.Framework;
using Umbraco.Core;
using uMigrations.Tests.Models.Db;

namespace uMigrations.Tests
{
    public class Db_Schema_Created
    {
        private DbTableRepository _repo = new DbTableRepository(ApplicationContext.Current.DatabaseContext.Database);

        [Test]
        public void When_Test_Started_Db_Schema_Created()
        {
            _repo.CreateSchema();

            var tables = _repo.GetAll();
            Assert.That(tables.Count > 0);
        }

        [TearDown]
        public void Cleanup()
        {
            _repo.DeleteAll();
        }
    }
}