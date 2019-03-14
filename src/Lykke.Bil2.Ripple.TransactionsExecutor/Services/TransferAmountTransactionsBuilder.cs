using System.Linq;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.TransactionsExecutor.Requests;
using Lykke.Bil2.Contract.TransactionsExecutor.Responses;
using Lykke.Bil2.Sdk.TransactionsExecutor.Services;

namespace Lykke.Bil2.Ripple.TransactionsExecutor.Services
{
    public class TransferAmountTransactionsBuilder : ITransferAmountTransactionsBuilder
    {
        public TransferAmountTransactionsBuilder(/* TODO: Provide specific settings and dependencies, if necessary */)
        {
        }

        public async Task<BuildTransactionResponse> BuildTransferAmountAsync(BuildTransferAmountTransactionRequest request)
        {
            // TODO: prepare data required for signing, return as base58 encoded string
            //
            // Throw:
            // - Lykke.Bil2.Contract.Common.RequestValidationException:
            //     if a transaction can’t be built with the given parameters and
            //     it will be never possible to build the transaction with exactly the same parameters.
            //
            // - Lykke.Bil2.Sdk.TransactionsExecutor.Exceptions.SendingTransactionBuildingException:
            //     if a transaction cannot be built and the reason can be mapped
            //     to the Lykke.Bil2.Contract.TransactionsExecutor.SendingTransactionBuildingError
            //
            // - Lykke.Bil2.Sdk.Exceptions.OperationNotSupportedException:
            //     if "transfer amount" transactions model is not supported by the blockchain.
            //
            // - System.Exception:
            //     if there are any other errors.
            //     Likely a temporary issue with infrastructure or configuration,
            //     request should be repeated later.





            throw new System.NotImplementedException();
        }
    }
}