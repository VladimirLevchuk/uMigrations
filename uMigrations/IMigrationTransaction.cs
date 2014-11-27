using System;

namespace uMigrations
{
    public interface IMigrationTransaction : IDisposable
    {
        bool Finished { get; }
        void Commit();
        void Rollback();
    }
}