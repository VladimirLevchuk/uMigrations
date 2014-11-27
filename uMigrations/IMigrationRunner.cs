namespace uMigrations
{
    public interface IMigrationRunner
    {
        void Apply(IMigration migration);
        bool IsMigrationStepApplied(IMigrationStep step);
    }
}