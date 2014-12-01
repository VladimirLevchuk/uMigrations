using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Persistence;

namespace uMigrations.Tests.Models.Db
{
    public class DbTableRepository
    {
        public DbTableRepository(Database db)
        {
            Db = db;
        }

        public Database Db { get; private set; }

        public void CreateSchema()
        {
            Db.CreateDatabaseSchema(false);
        }

        public List<DbTable> GetAll()
        {
            var result = Db.Query<DbTable>("SELECT [name] FROM sys.tables").ToList();
            return result;
        }

        public void DeleteAll()
        {
            var sql = @"
SELECT fk.[name], tableName = o.[name], schemaName = s.[name]   
FROM [sys].[foreign_keys] fk
JOIN sys.all_objects o
	ON o.object_id = fk.parent_object_id
JOIN sys.schemas s
	ON s.schema_id = o.schema_id	
";

            var foreignKeys = Db.Query<DbForeignKey>(sql).ToList();

            foreach (var foreignKey in foreignKeys)
            {
                var removeFkSql = string.Format("ALTER TABLE [{0}].[{1}] DROP CONSTRAINT [{2}]",
                    foreignKey.SchemaName, foreignKey.TableName, foreignKey.Name);

                Db.Execute(new Sql(removeFkSql));
            }

            var tables = GetAll();

            foreach (var table in tables)
            {
                Db.DropTable(table.Name);
            }
        }
    }
}
