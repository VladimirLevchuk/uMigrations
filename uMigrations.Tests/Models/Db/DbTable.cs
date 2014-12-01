namespace uMigrations.Tests.Models.Db
{
    public class DbTable
    {
        public string Name { get; set; }
    }

    public class DbForeignKey
    {
        public string Name { get; set; }

        public string TableName { get; set; }

        public string SchemaName { get; set; }
    }
}