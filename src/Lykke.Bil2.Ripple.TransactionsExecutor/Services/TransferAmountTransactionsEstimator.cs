using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Contract.TransactionsExecutor.Requests;
using Lykke.Bil2.Contract.TransactionsExecutor.Responses;
using Lykke.Bil2.Ripple.Client;
using Lykke.Bil2.Ripple.Client.Api.ServerState;
using Lykke.Bil2.Sdk.TransactionsExecutor.Services;
using Lykke.Numerics;

namespace Lykke.Bil2.Ripple.TransactionsExecutor.Services
{
    public class TransferAmountTransactionsEstimator : ITransferAmountTransactionsEstimator
    {
        private readonly IRippleApi _rippleApi;
        private readonly decimal _feeFactor;
        private readonly decimal _maxFee;

        public TransferAmountTransactionsEstimator(
            IRippleApi rippleApi,
            decimal? feeFactor = null,
            decimal? maxFee = null)
        {
            _rippleApi = rippleApi;
            _feeFactor = feeFactor ?? 1.2M;
            _maxFee = maxFee ?? 2M;
        }

        public async Task<EstimateTransactionResponse> EstimateTransferAmountAsync(EstimateTransferAmountTransactionRequest request)
        {
            var serverStateResponse = await _rippleApi.Post(new ServerStateRequest());

            serverStateResponse.Result.ThrowIfError();

            var fee = Math.Min
            (
                _feeFactor * (serverStateResponse.Result.State.GetFee() / 1_000_000M),
                _maxFee
            );

            return new EstimateTransactionResponse
            (
                new Dictionary<AssetId, UMoney>
                {
                    ["XRP"] = UMoney.Create(fee, 6)
                }
            );
        }
    }
}