namespace uMigrations.Persistance
{
    public interface IMigrationInfoRepository
    {
        MigrationInfo Insert(MigrationInfo info);
        MigrationInfo SingleOrDefault(string stepName);
    }
}