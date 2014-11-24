using System;
using System.Collections.Generic;
using System.Linq;
using log4net;

namespace uMigrations
{
    public class ManualMigrationRunner : IMigrationRunner
    {
        protected IMigrationTransactionProvider MigrationTransactionProvider { get; private set; }
        protected IContentMigrationService ContentMigrationService { get; private set; }
        protected ILog Log { get; private set; }

        public ManualMigrationRunner(ILog log, IMigrationTransactionProvider migrationTransactionProvider, IContentMigrationService contentMigrationService)
        {
            ContentMigrationService = contentMigrationService;
            Log = log;
            MigrationTransactionProvider = migrationTransactionProvider;
        }

        public virtual bool IsMigrationApplied(IMigration migration)
        {
            throw new NotImplementedException();
        }

        public virtual void Upgrade(IMigration migration)
        {
            Run(migration, x => x.ApplyActions);
        }

        public virtual void Downgrade(IMigration migration)
        {
            Run(migration, x => x.RemoveActions);
        }

        public virtual void Run(IMigration migration, Func<IMigrationStep, IEnumerable<IMigrationAction>> migrationDirection)
        {
            if (MigrationContext.Current.MigrationSettings.SkipMigrations)
            {
                return;
            }

            using (var tran = MigrationContext.Current.TransactionProvider.BeginTransaction())
            {
                var steps = migration.MigrationSteps;

                var problems = new List<Exception>();

                foreach (var step in steps)
                {
                    foreach (var migrationAction in migrationDirection(step))
                    {
                        var migrationProblems = migrationAction.Validate();

                        if (migrationProblems.Count > 0)
                        {
                            var message = string.Format("Migration error. Step: '{0}', Action: '{1}'", 
                                step, migrationAction); 

                            problems.Add(new AggregateException(message, migrationProblems));
                        }
                    }
                }

                if (problems.Any())
                {
                    var message = "Migration error(s)";
                    throw new AggregateException(message, problems);
                }

                try
                {
                    // steps.SelectMany(migrationDirection).ToList().ForEach(x => x.Run());
                    foreach (var step in steps)
                    {
                        foreach (var migrationAction in migrationDirection(step))
                        {
                            migrationAction.Run();
                        }
                    }

                    ContentMigrationService.RepublishAllContent();
                    
                    tran.Commit();
                }
                catch (Exception error)
                {
                    tran.Rollback();
                    Log.Error(error);
                    throw;
                }
            }
        }
    }
}