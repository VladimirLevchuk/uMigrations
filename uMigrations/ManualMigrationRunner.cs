using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using uMigrations.Persistance;

namespace uMigrations
{
    public class ManualMigrationRunner : IMigrationRunner
    {
        protected IMigrationTransactionProvider MigrationTransactionProvider { get; private set; }
        protected IContentMigrationService ContentMigrationService { get; private set; }
        protected IMigrationInfoRepository MigrationInfoRepository { get; private set; }
        protected IMigrationSettings MigrationSettings { get; private set; }
        protected ILog Log { get; private set; }

        protected Database Db // todo: use injection
        {
            get { return ApplicationContext.Current.DatabaseContext.Database; }
        }

        public ManualMigrationRunner(ILog log, 
            IMigrationTransactionProvider migrationTransactionProvider, 
            IContentMigrationService contentMigrationService,
            IMigrationInfoRepository migrationInfoRepository,
            IMigrationSettings migrationSettings)
        {
            MigrationSettings = migrationSettings;
            ContentMigrationService = contentMigrationService;
            MigrationInfoRepository = migrationInfoRepository;
            Log = log;
            MigrationTransactionProvider = migrationTransactionProvider;
        }

        public virtual void Apply(IMigration migration)
        {
            var migrationSettings = MigrationContext.Current.MigrationSettings;
            if (migrationSettings.SkipMigrations)
            {
                return;
            }

            using (var tran = MigrationContext.Current.TransactionProvider.BeginTransaction())
            {
                var steps = migration.MigrationSteps.Where(x => IsMigrationStepApplied(x) == false).ToList();

                var problems = Verify(steps);

                if (problems.Any())
                {
                    var message = "Migration error(s)";
                    throw new AggregateException(message, problems);
                }

                try
                {
                    foreach (var step in steps)
                    {
                        foreach (var migrationAction in step.ApplyActions)
                        {
                            migrationAction.Run();
                        }

                        PersistMigrationStepAppliedInformation(step);
                    }

                    if (migrationSettings.EmulateMigrations)
                    {
                        tran.Rollback();
                    }
                    else
                    {
                        tran.Commit();
                    }
                }
                catch (Exception error)
                {
                    tran.Rollback();
                    Log.Error(error);
                    throw;
                }

                ContentMigrationService.RepublishAllContent();
            }
        }

        public virtual bool IsMigrationStepApplied(IMigrationStep step)
        {
            var appliedMigrationStep = MigrationInfoRepository.SingleOrDefault(step.Name);
            var result = appliedMigrationStep != null;
            return result;
        }
           
        private void PersistMigrationStepAppliedInformation(IMigrationStep step)
        {
            var info = new MigrationInfo
            {
                MigrationStepName = step.Name,
                Applied = DateTime.UtcNow,
                Suffix = MigrationSettings.MigrationRuntimeId
            };

            MigrationInfoRepository.Insert(info);
        }

        protected virtual List<Exception> Verify(List<IMigrationStep> steps)
        {
            var result = new List<Exception>();

            foreach (var step in steps)
            {
                if (IsMigrationStepApplied(step) /*|| !step.IsApplicable*/)
                {
                    continue;
                }

                foreach (var migrationAction in step.ApplyActions)
                {
                    var migrationProblems = migrationAction.Validate();

                    if (migrationProblems.Count > 0)
                    {
                        var message = string.Format("Migration error. Step: '{0}', Action: '{1}'",
                            step, migrationAction);

                        result.Add(new AggregateException(message, migrationProblems));
                    }
                }
            }

            return result;
        }
    }
}