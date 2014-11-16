using System;
using System.Net.Configuration;
using Umbraco.Core.Persistence;

namespace uMigrations
{
    public class MigrationTransaction : IMigrationTransaction, ITransactionStateChangedNotifier
    {
        private Transaction _transaction;
        private readonly object SyncRoot = new object();

        public MigrationTransaction(Transaction transaction)
        {
            _transaction = transaction;
        }

        public virtual bool Finished
        {
            get { return _transaction == null; }
        }

        private void DisposeTransaction(bool complete)
        {
            lock (SyncRoot)
            {
                if (complete)
                {
                    _transaction.Complete();
                }

                _transaction.Dispose();
                _transaction = null;
            }

            OnTransactionStateChanged(new TransactionStateChangedEventArgs { Completed = complete });
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !Finished)
            {
                DisposeTransaction(false);
            }
        }

        public virtual void Commit()
        {
            DisposeTransaction(true);
        }

        public virtual void Rollback()
        {
            DisposeTransaction(false);
        }

        public event EventHandler<TransactionStateChangedEventArgs> TransactionStateChanged;

        protected virtual void OnTransactionStateChanged(TransactionStateChangedEventArgs e)
        {
            EventHandler<TransactionStateChangedEventArgs> handler = TransactionStateChanged;
            if (handler != null) handler(this, e);
        }
    }
}