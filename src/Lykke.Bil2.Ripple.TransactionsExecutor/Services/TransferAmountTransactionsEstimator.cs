using System.Numerics;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.TransactionsExecutor.Requests;
using Lykke.Bil2.Contract.TransactionsExecutor.Responses;
using Lykke.Bil2.Ripple.Client;
using Lykke.Bil2.Ripple.Client.Api.ServerState;
using Lykke.Bil2.Sdk.TransactionsExecutor.Services;
using Lykke.Numerics;
using Lykke.Bil2.SharedDomain;

namespace Lykke.Bil2.Ripple.TransactionsExecutor.Services
{
    public class TransferAmountTransactionsEstimator : ITransferAmountTransactionsEstimator
    {
        private readonly IRippleApi _rippleApi;
        private readonly decimal _feeFactor;

        public TransferAmountTransactionsEstimator(IRippleApi rippleApi, decimal? feeFactor = null)
        {
            _rippleApi = rippleApi;
            _feeFactor = feeFactor ?? 1.2M; // to be confident fee is enough
        }

        public async Task<EstimateTransactionResponse> EstimateTransferAmountAsync(EstimateTransferAmountTransactionRequest request)
        {
            var serverStateResponse = await _rippleApi.Post(new ServerStateRequest());

            serverStateResponse.Result.ThrowIfError();

            var fee = new Fee
            (
                new Asset("XRP"),
                new UMoney(new BigInteger(serverStateResponse.Result.State.GetFee() * _feeFactor), 6)
            );

            return new EstimateTransactionResponse(new [] { fee });
        }
    }
}