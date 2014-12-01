using System.Linq;
using NUnit.Framework;
using umbraco.cms.businesslogic;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using uMigrations.Tests.Models.Db;

namespace uMigrations.Tests
{
    public class Db_Schema_Created
    {
        private DbTableRepository _repo = new DbTableRepository(ApplicationContext.Current.DatabaseContext.Database);

        [Test]
        public void When_Test_Started_Db_Schema_Created()
        {
            var tables = _repo.GetAll();
            Assert.That(tables.Count > 0);
        }
    }
}