using System;
using System.Diagnostics;
using log4net;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.UnitOfWork;
using Umbraco.Core.Services;

namespace uMigrations
{
    public class MigrationContext
    {
        public virtual MigrationsSettings MigrationSettings { get; private set; }
        public virtual IContentMigrationService ContentMigrationService { get; private set; }
        public virtual IMigrationTransactionProvider TransactionProvider { get; private set; }
        public virtual IMigrationsApi Api { get; private set; }
        public virtual Func<Type, ILog> LogFactoryMethod { get; private set; }
        // public virtual IMigrationProvider MigrationProvider { get; private set; }
        public virtual IMigrationRunner Runner { get; private set; }

        public static MigrationContext Current = CreateDefaultContext();

        public MigrationContext(MigrationsSettings migrationSettings, IContentMigrationService contentMigrationService, IMigrationTransactionProvider transactionProvider, IMigrationsApi api, Func<Type, ILog> logFactoryMethod, IMigrationRunner runner)
        {
            Runner = runner;
            MigrationSettings = migrationSettings;
            ContentMigrationService = contentMigrationService;
            TransactionProvider = transactionProvider;
            Api = api;

            LogFactoryMethod = logFactoryMethod;
        }

        static MigrationContext CreateDefaultContext()
        {
            var dbContext = ApplicationContext.Current.DatabaseContext;
            // var services = ApplicationContext.Current.Services;
            var migrationSettings = new MigrationsSettings();

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

            var runner = new ManualMigrationRunner(logFactoryMethod(typeof(MigrationsApi)), transactionProvider, contentMigrationService);
            //var api = new MigrationsApi(contentMigrationService, transactionProvider,
            //    logFactoryMethod(typeof (MigrationsApi)));

            var result = new MigrationContext(migrationSettings, 
                contentMigrationService, transactionProvider, null, logFactoryMethod, runner);
            return result;
        }
    }
}