using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace uMigrations.Persistance
{
    [TableName("uMigrationStep")]
    [PrimaryKey("id")]
    [ExplicitColumns]
    public class MigrationInfo
    {
        [Column("id")]
        [PrimaryKeyColumn(Name = "PK_MigrationStep", AutoIncrement = true)]
        public int Id { get; set; }

        [Column("stepName")]
        [Length(4000)]
        public string MigrationStepName { get; set; }

        //[Column("version")]
        //[Length(50)]
        //public string Version { get; set; }
        
        [Column("applied")]
        public DateTime Applied { get; set; }

        [Column("suffix")]
        [Length(50)]
        public string Suffix { get; set; }
    }
}