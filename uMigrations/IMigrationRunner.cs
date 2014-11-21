namespace uMigrations
{
    public interface IMigrationRunner
    {
        void Upgrade(IMigration migration);
        void Downgrade(IMigration migration);
    }
}