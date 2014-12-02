namespace uMigrations.Tests.features.Migrations
{
    public class MigrationsSettings : IMigrationSettings
    {
        public int SystemUserId { get; set; }
        public bool SkipMigrations { get; set; }
        public bool EmulateMigrations { get; set; }
        public string MigrationRuntimeId { get; set; }
    }
}