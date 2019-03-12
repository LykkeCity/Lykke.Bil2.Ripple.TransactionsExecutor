using System.Linq;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.TransactionsExecutor.Requests;
using Lykke.Bil2.Contract.TransactionsExecutor.Responses;
using Lykke.Bil2.Sdk.TransactionsExecutor.Services;

namespace Lykke.Bil2.Ripple.TransactionsExecutor.Services
{
    public class TransferAmountTransactionsEstimator : ITransferAmountTransactionsEstimator
    {
        public TransferAmountTransactionsEstimator(/* TODO: Provide specific settings and dependencies, if necessary */)
        {
        }

        public async Task<EstimateTransactionResponse> EstimateTransferAmountAsync(EstimateTransferAmountTransactionRequest request)
        {
            // TODO: estimate transaction fees.
            //
            // Throw:
            // - Lykke.Bil2.Contract.Common.RequestValidationException:
            //     if a transaction can’t be estimated with the given parameters
            //     and it will be never possible to estimate the transaction with exactly the same
            //     parameters.
            //
            // - System.Exception:
            //     if there are any other errors.
            //     Likely a temporary issue with infrastructure or configuration,
            //     request should be repeated later.
            //
            // Result usually consists of single fee element,
            // but for some blockchains fees may be charged in multiple currencies/tokens.
            //
            // For example:
            //
            // var fee = ...;
            //
            // return new EstimateSendingTransactionResponse
            // (
            //     new Dictionary<AssetId, CoinsAmount>
            //     {
            //         { "BTC", CoinsAmount.FromDecimal(fee, accuracy: 8) }
            //     }
            // );


            throw new System.NotImplementedException();
        }
    }
}