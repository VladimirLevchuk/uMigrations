using Umbraco.Core;
using uMigrations.Tests.Models.Db;

namespace uMigrations.Tests
{
    public class TestEnvironment
    {
        private TestApplication _application = new TestApplication();
        private DbTableRepository _repo;

        public void Startup()
        {
            _application.Start();
            _repo = new DbTableRepository(ApplicationContext.Current.DatabaseContext.Database);
            // _repo.CreateSchema();
        }

        public void Shutdown()
        {
            // _repo.DeleteAll();
            _application.Finish();
        }
    }
}