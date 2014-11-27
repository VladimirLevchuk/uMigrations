using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Umbraco.Core;

namespace uMigrations
{
    public interface IMigrationTransactionProvider
    {
        IMigrationTransaction BeginTransaction();

        bool IsTransactionStarted();
    }

    public class MigrationTransactionProvider : IMigrationTransactionProvider
    {
        protected DatabaseContext Context { get; set; }
        private readonly ConcurrentDictionary<IMigrationTransaction, bool> _transactions = new ConcurrentDictionary<IMigrationTransaction, bool>();

        public MigrationTransactionProvider(DatabaseContext context)
        {
            Context = context;
        }

        public virtual IMigrationTransaction BeginTransaction()
        {
            var result = new MigrationTransaction(Context.Database.GetTransaction());
            result.TransactionStateChanged += TransactionStateChanged;
            if (!result.Finished) // hypotatically transaction could be rolled back at this point 
            {
                _transactions.TryAdd(result, true);
            }
            return result;
        }

        private void TransactionStateChanged(object sender, TransactionStateChangedEventArgs e)
        {
            var transaction = (IMigrationTransaction) sender;
            bool foo;
            _transactions.TryRemove(transaction, out foo);
        }

        public virtual bool IsTransactionStarted()
        {
            return _transactions.Any();
        }
    }
}