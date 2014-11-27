using System;
using Umbraco.Core.Persistence;

namespace uMigrations.Persistance
{
    public class PetaPocoMigrationInfoRepository : IMigrationInfoRepository
    {
        private readonly Database _db;

        public PetaPocoMigrationInfoRepository(Database db)
        {
            _db = db;
        }

        public virtual MigrationInfo Insert(MigrationInfo info)
        {
            var newId = _db.Insert(info);
            info.Id = Convert.ToInt32(newId);
            return info;
        }

        public virtual void Delete(int id)
        {
            var temp = _db.Delete<MigrationInfo>(id);
        }

        public virtual void Update(MigrationInfo info)
        {
            var temp = _db.Update(info);
        }

        public virtual MigrationInfo GetById(int id)
        {
            var result = _db.Single<MigrationInfo>(id);
            return result;
        }

        public virtual MigrationInfo SingleOrDefault(string stepName)
        {
            var sql = new Sql("WHERE stepName=@0", stepName);
            var result = _db.SingleOrDefault<MigrationInfo>(sql);
            return result;
        }

        public static void AppStart(Database db)
        {
            using (var tran = db.GetTransaction())
            {
                if (!db.TableExist("uMigrationStep"))
                {
                    db.CreateTable<MigrationInfo>();
                }

                tran.Complete();
            }
        }
    }
}