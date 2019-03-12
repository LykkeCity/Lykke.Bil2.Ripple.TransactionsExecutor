using System.Threading.Tasks;
using Lykke.Bil2.Contract.TransactionsExecutor;
using Lykke.Bil2.Sdk.TransactionsExecutor.Services;

namespace Lykke.Bil2.Ripple.TransactionsExecutor.Services
{
    public class TransactionsStateProvider : ITransactionsStateProvider
    {
        public TransactionsStateProvider(/* TODO: Provide specific settings and dependencies, if necessary */)
        {
        }

        public async Task<TransactionState> GetStateAsync(string transactionId)
        {
            // TODO: Check and return transaction state.
            //
            // Memory pool should be checked first to exclude transaction loss due to race condition.
            // Use TransactionState.Accepted state if transaction is added/enqueued
            // to the mempool but still not broadcasted to the network.
            //
            // For example:
            //
            // var tx = ...;
            //
            // if (tx == null)
            // {
            //     return TransactionState.Unknown;
            // }
            //
            // if (tx.BlockId = null)
            // {
            //     return TransactionState.Broadcasted;
            // }
            // else
            // {
            //     return TransactionState.Mined;
            // }


            throw new System.NotImplementedException();
        }
    }
}