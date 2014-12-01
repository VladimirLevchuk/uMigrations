using NUnit.Framework;

namespace uMigrations.Tests
{
    [SetUpFixture]
    public class TestAppStart
    {
        TestEnvironment _environment;

        [SetUp]
        public void BeforeAll()
        {
            _environment = new TestEnvironment();
            _environment.Startup();
        }

        [TearDown]
        public void AfterAll()
        {
            _environment.Shutdown();
        }
    }
}