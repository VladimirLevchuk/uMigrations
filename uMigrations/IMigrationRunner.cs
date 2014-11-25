namespace uMigrations
{
    public interface IMigrationRunner
    {
        void Apply(IMigration migration);
        void Remove(IMigration migration);
    }
}