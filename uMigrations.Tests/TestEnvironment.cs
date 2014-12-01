using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace uMigrations.Tests
{
    public class TestEnvironment
    {
        private TestApplication _application = new TestApplication();

        public void Startup()
        {
            _application.Start();

            var db = ApplicationContext.Current.DatabaseContext.Database;
            db.CreateDatabaseSchema(true);
        }

        public void Shutdown()
        {
            // todo: ?remove all db tables
        }
    }
}