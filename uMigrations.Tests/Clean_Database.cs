using NUnit.Framework;
using uMigrations.Tests.Models.Db;

namespace uMigrations.Tests
{
    [Ignore]
    public class Clean_Database : TestBase
    {
        [Test]
        public void Please()
        {
            var repo = new DbTableRepository(Db);

            repo.DeleteAll();
        }
    }
}