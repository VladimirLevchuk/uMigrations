using System;
using Umbraco.Core;
using Umbraco.Web;

namespace uMigrations
{
    public class TransactionStateChangedEventArgs : EventArgs
    {
        public bool Completed { get; set; }
    }

    public interface ITransactionStateChangedNotifier
    {
        event EventHandler<TransactionStateChangedEventArgs> TransactionStateChanged;
    }

    public interface IMigrationTransaction : IDisposable
    {
        bool Finished { get; }
        void Commit();
        void Rollback();
    }
}