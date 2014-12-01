using NUnit.Framework;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace uMigrations.Tests
{
    public abstract class TestBase
    {
        protected Database Db 
        { 
            get { return ApplicationContext.Current.DatabaseContext.Database; }
        }

        [TestFixtureSetUp]
        protected virtual void BeforeAll()
        {}

        [TestFixtureTearDown]
        protected virtual void AfterAll()
        {}

        [SetUp]
        protected virtual void BeforeEach()
        {}

        [TearDown]
        protected virtual void AfterEach()
        {}
    }
}