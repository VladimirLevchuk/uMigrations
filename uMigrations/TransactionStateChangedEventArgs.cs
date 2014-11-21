using System;

namespace uMigrations
{
    public class TransactionStateChangedEventArgs : EventArgs
    {
        public bool Completed { get; set; }
    }
}