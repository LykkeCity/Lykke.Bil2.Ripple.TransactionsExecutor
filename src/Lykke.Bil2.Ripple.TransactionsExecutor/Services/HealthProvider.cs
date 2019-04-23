using System.Threading.Tasks;
using Lykke.Bil2.Ripple.Client;
using Lykke.Bil2.Ripple.Client.Api.ServerState;
using Lykke.Bil2.Sdk.TransactionsExecutor.Services;
using Refit;

namespace Lykke.Bil2.Ripple.TransactionsExecutor.Services
{
    public class HealthProvider : IHealthProvider
    {
        private readonly IRippleApi _rippleApi;

        public HealthProvider(IRippleApi rippleApi)
        {
            _rippleApi = rippleApi;
        }

        public async Task<string> GetDiseaseAsync()
        {
            RippleResponse<ServerStateResult> serverStateResponse;

            try
            {
                serverStateResponse = await _rippleApi.Post(new ServerStateRequest());
            }
            catch (ApiException ex)
            {
                return $"Node is unavailable: {ex}";
            }

            if (serverStateResponse.Result.Status == "error")
            {
                return $"Node state request error: {serverStateResponse.Result.Error}";
            }

            if (serverStateResponse.Result.State.ServerState != "full" &&
                serverStateResponse.Result.State.ServerState != "validating" &&
                serverStateResponse.Result.State.ServerState != "proposing")
            {
                return $"Node state is unexpected: {serverStateResponse.Result.State.ServerState}";
            }

            return null;
        }
    }
}
