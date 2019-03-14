using System.Threading.Tasks;
using Lykke.Bil2.Contract.TransactionsExecutor;
using Lykke.Bil2.Ripple.Client;
using Lykke.Bil2.Ripple.Client.Api.Tx;
using Lykke.Bil2.Sdk.TransactionsExecutor.Services;

namespace Lykke.Bil2.Ripple.TransactionsExecutor.Services
{
    public class TransactionsStateProvider : ITransactionsStateProvider
    {
        private readonly IRippleApi _rippleApi;

        public TransactionsStateProvider(IRippleApi rippleApi)
        {
            _rippleApi = rippleApi;
        }

        public async Task<TransactionState> GetStateAsync(string transactionId)
        {
            var txResponse = await _rippleApi.Post(new TxRequest(transactionId));

            if (txResponse.Result.Error == "txnNotFound")
            {
                return TransactionState.Unknown;
            }

            txResponse.Result.ThrowIfError();

            return txResponse.Result.Validated.HasValue && txResponse.Result.Validated.Value
                ? TransactionState.Mined
                : TransactionState.Broadcasted;
        }
    }
}