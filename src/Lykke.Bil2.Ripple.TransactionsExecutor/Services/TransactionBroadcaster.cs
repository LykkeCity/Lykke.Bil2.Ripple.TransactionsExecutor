using System.Threading.Tasks;
using Lykke.Bil2.Contract.TransactionsExecutor.Requests;
using Lykke.Bil2.Sdk.TransactionsExecutor.Services;

namespace Lykke.Bil2.Ripple.TransactionsExecutor.Services
{
    public class TransactionBroadcaster : ITransactionBroadcaster
    {
        public TransactionBroadcaster(/* TODO: Provide specific settings and dependencies, if necessary */)
        {
        }

        public async Task BroadcastAsync(BroadcastTransactionRequest request)
        {
            // TODO: broadcast transaction
            //
            // Throw:
            // - Lykke.Bil2.Contract.Common.RequestValidationException:
            //     if a transaction can’t be broadcasted with the given parameters.
            //     It should be guaranteed that the transaction is not included and will not be
            //     included to the any blockchain block.
            //
            // - Lykke.Bil2.Sdk.TransactionsExecutor.Exceptions.TransactionBroadcastingException:
            //     if a transaction cannot be broadcasted and the reason can be
            //     mapped to the Lykke.Bil2.Contract.TransactionsExecutor.TransactionBroadcastingError.
            //     It should be guaranteed that the transaction is not included and will not be
            //     included to the any blockchain block.
            //
            // - System.Exception:
            //     if there are any other errors.
            //     It can be thrown without guarantee that transaction was broadcasted to the
            //     blockchain or not. Request will be not retried automatically.
            //
            // For example
            //
            // SignedTransaction signed;
            //
            // try
            // {
            //     signed = JsonConvert.DeserializeObject<SignedTransaction>(request.SignedTransaction.DecodeToString());
            // }
            // catch (JsonException ex)
            // {
            //     throw new RequestValidationException("Failed to deserialize signed transaction", request.SignedTransaction, ex, nameof(request.SignedTransaction));
            // }
            //
            // var result = await BcnApi.Send(signed);
            //
            // switch (result)
            // {
            //     case 100:
            //         throw new TransactionBroadcastingException(TransactionBroadcastingError.RebuildRequired, "Transaction expired");
            //
            //     case 200:
            //         throw new TransactionBroadcastingException(TransactionBroadcastingError.RetryLater, "Node is busy");
            // }


            throw new System.NotImplementedException();
        }
    }
}