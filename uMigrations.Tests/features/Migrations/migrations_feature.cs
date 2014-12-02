using System;
using uMigrations.Tests.features.MovePropertyUp;
using uMigrations.Tests.Models.Db;
using Vega.USiteBuilder.DocumentTypeBuilder.Contracts;
using Vega.USiteBuilder.Repositories;

namespace uMigrations.Tests.features.Migrations
{
    public abstract class migrations_feature : feature
    {
        protected MigrationContext MigrationContext
        {
            get { return MigrationContext.Current; }
            set { MigrationContext.Current = value; }
        }

        protected void RunMigration(string name, Action<FluentMigrationStepDefinition> define)
        {
            var migration = new FluentMigrationStepDefinition();
            define(migration);
            var step = new TestMigrationStep(name, migration.GetActions());
            MigrationContext.Runner.Apply(new CustomMigration(step));
        }

        protected void AddDocumentTypes(params Type[] types)
        {
            new TestDocumentTypeManager().SynchronizeDocumentTypes(types);
        }

        private DbTableRepository _repo;

        protected void InitializeDb()
        {
            _repo = new DbTableRepository(Db);
        }

        protected void FinalizeDb()
        {
            _repo = null;
        }

        protected void CreateDbSchema()
        {
            _repo.CreateSchema();
        }

        protected void DeleteDbSchema()
        {
            _repo.DeleteAll();
        }

        protected void SetupMigrationContext(string runtimeId, bool emulate = false, bool skip = false, int systemUserId = 0)
        {
            MigrationContext = MigrationTestUtil.CreateMigrationContext(new MigrationsSettings
            {
                SystemUserId = systemUserId,
                EmulateMigrations = emulate,
                MigrationRuntimeId = runtimeId,
                SkipMigrations = skip
            });

            DocumentRepository.SetCurrent(new Framework.DocumentRepository(NodeRepository.Current, 
                MigrationContext.ContentService));
        }
    }
}