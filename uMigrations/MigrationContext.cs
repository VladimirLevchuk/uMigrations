using System;
using log4net;
using Umbraco.Core;

namespace uMigrations
{
    public class MigrationContext
    {
        public virtual IContentMigrationService ContentMigrationService { get; private set; }
        public virtual IMigrationTransactionProvider TransactionProvider { get; private set; }
        public virtual IMigrationsApi Api { get; private set; }
        public virtual Func<Type, ILog> LogFactoryMethod { get; private set; }

        public static MigrationContext Current = CreateDefaultContext();

        public MigrationContext(IContentMigrationService contentMigrationService, 
            IMigrationTransactionProvider transactionProvider,
            IMigrationsApi api, Func<Type, ILog> logFactoryMethod)
        {
            ContentMigrationService = contentMigrationService;
            TransactionProvider = transactionProvider;
            Api = api;
            LogFactoryMethod = logFactoryMethod;
        }

        static MigrationContext CreateDefaultContext()
        {
            var dbContext = ApplicationContext.Current.DatabaseContext;
            var services = ApplicationContext.Current.Services;

            var contentMigrationService = new ContentMigrationService(services.ContentTypeService,
                services.ContentService);
            var transactionProvider = new MigrationTransactionProvider(dbContext);
            Func<Type, ILog> logFactoryMethod = LogManager.GetLogger;

            var api = new MigrationsApi(contentMigrationService, transactionProvider,
                logFactoryMethod(typeof (MigrationsApi)));

            var result = new MigrationContext(contentMigrationService, transactionProvider, api, logFactoryMethod);
            return result;
        }
    }
}