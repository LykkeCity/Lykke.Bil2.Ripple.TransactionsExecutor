using System.Threading.Tasks;
using Lykke.Bil2.Contract.TransactionsExecutor.Responses;
using Lykke.Bil2.Ripple.Client;
using Lykke.Bil2.Ripple.Client.Api.ServerState;
using Lykke.Bil2.Sdk.TransactionsExecutor.Services;

namespace Lykke.Bil2.Ripple.TransactionsExecutor.Services
{
    public class BlockchainInfoProvider : IBlockchainInfoProvider
    {
        private readonly IRippleApi _rippleApi;

        public BlockchainInfoProvider(IRippleApi rippleApi)
        {
            _rippleApi = rippleApi;
        }

        public async Task<BlockchainInfo> GetInfoAsync()
        {
            var serverStateResponse = await _rippleApi.Post(new ServerStateRequest());

            serverStateResponse.Result.ThrowIfError();

            var lastLedger =
                serverStateResponse.Result.State.ClosedLedger ??
                serverStateResponse.Result.State.ValidatedLedger;

            return new BlockchainInfo(lastLedger.Seq, lastLedger.CloseTime.FromRippleEpoch());
        }
    }
}
