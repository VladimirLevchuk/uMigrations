using System;

namespace uMigrations
{
    public interface ITransactionStateChangedNotifier
    {
        event EventHandler<TransactionStateChangedEventArgs> TransactionStateChanged;
    }
}