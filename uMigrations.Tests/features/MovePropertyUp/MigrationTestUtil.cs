using System;
using System.Diagnostics;
using log4net;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.UnitOfWork;
using Umbraco.Core.Services;
using uMigrations.Persistance;

namespace uMigrations.Tests.features.MovePropertyUp
{
    public class MigrationsSettings : IMigrationSettings
    {
        public int SystemUserId { get; set; }
        public bool SkipMigrations { get; set; }
        public bool EmulateMigrations { get; set; }
        public string MigrationRuntimeId { get; set; }
    }

    public class MigrationTestUtil
    {
        public static MigrationContext CreateMigrationContext(MigrationsSettings migrationSettings)
        {
            var dbContext = ApplicationContext.Current.DatabaseContext;

            var repositoryFactory = new RepositoryFactory(disableAllCache: true);

            var unitOfWorkProvider = new PetaPocoUnitOfWorkProvider();
            using (var uow = unitOfWorkProvider.GetUnitOfWork())
            {
                Debug.Assert(uow.Database == dbContext.Database,
                    "Nested Transactions work only when all operations use the same Database instace. ");
            }

            var contentService = new ContentService(unitOfWorkProvider, repositoryFactory);
            var contentTypeService = new ContentTypeService(unitOfWorkProvider, repositoryFactory, contentService, new MediaService(repositoryFactory));
            var dataTypeService = new DataTypeService(unitOfWorkProvider, repositoryFactory);

            var contentMigrationService = new ContentMigrationService(contentTypeService,
                contentService, dataTypeService, migrationSettings);
            var transactionProvider = new MigrationTransactionProvider(dbContext);
            Func<Type, ILog> logFactoryMethod = LogManager.GetLogger;

            // add migration info to db if needed
            PetaPocoMigrationInfoRepository.AppStart(dbContext.Database);

            var migrationInfoRepository = new PetaPocoMigrationInfoRepository(dbContext.Database);

            var runner = new ManualMigrationRunner(logFactoryMethod(typeof(ManualMigrationRunner)), transactionProvider, contentMigrationService,
                migrationInfoRepository, migrationSettings);

            var result = new MigrationContext(migrationSettings,
                contentMigrationService, transactionProvider, logFactoryMethod, runner);
            return result;
        }        
    }
}