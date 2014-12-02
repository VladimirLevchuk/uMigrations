using System;
using System.Diagnostics;
using log4net;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.UnitOfWork;
using Umbraco.Core.Services;
using uMigrations.Persistance;

namespace uMigrations
{
    public class MigrationContext
    {
        public virtual IMigrationSettings MigrationSettings { get; private set; }
        public virtual IContentMigrationService ContentMigrationService { get; private set; }
        public virtual IMigrationTransactionProvider TransactionProvider { get; private set; }
        public virtual Func<Type, ILog> LogFactoryMethod { get; private set; }
        public virtual IMigrationRunner Runner { get; private set; }
        public virtual IContentService ContentService { get; private set; }
        public virtual IContentTypeService ContentTypeService { get; private set; }

        public static MigrationContext Current
        {
            get { return _customContext ?? _currentContext.Value; }
            set { _customContext = value; }
        }

        private static MigrationContext _customContext = null;
        private static readonly Lazy<MigrationContext> _currentContext = new Lazy<MigrationContext>(CreateDefaultContext);

        public MigrationContext(IMigrationSettings migrationSettings, 
            IContentService contentService, 
            IContentTypeService contentTypeService,
            IContentMigrationService contentMigrationService, 
            IMigrationTransactionProvider transactionProvider, 
            Func<Type, ILog> logFactoryMethod, 
            IMigrationRunner runner)
        {
            ContentTypeService = contentTypeService;
            ContentService = contentService;
            Runner = runner;
            MigrationSettings = migrationSettings;
            ContentMigrationService = contentMigrationService;
            TransactionProvider = transactionProvider;

            LogFactoryMethod = logFactoryMethod;
        }

        static MigrationContext CreateDefaultContext()
        {
            var dbContext = ApplicationContext.Current.DatabaseContext;
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

            // add migration info to db if needed
            PetaPocoMigrationInfoRepository.AppStart(dbContext.Database);

            var migrationInfoRepository = new PetaPocoMigrationInfoRepository(dbContext.Database);

            var runner = new ManualMigrationRunner(logFactoryMethod(typeof(ManualMigrationRunner)), transactionProvider, contentMigrationService,
                migrationInfoRepository, migrationSettings);

            var result = new MigrationContext(migrationSettings, contentService, contentTypeService,
                contentMigrationService, transactionProvider, logFactoryMethod, runner);
            return result;
        }
    }
}