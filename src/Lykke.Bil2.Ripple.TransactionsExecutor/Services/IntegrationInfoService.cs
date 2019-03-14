using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Bil2.Contract.TransactionsExecutor.Responses;
using Lykke.Bil2.Ripple.Client;
using Lykke.Bil2.Ripple.Client.Api.ServerState;
using Lykke.Bil2.Sdk.Exceptions;
using Lykke.Bil2.Sdk.TransactionsExecutor.Models;
using Lykke.Bil2.Sdk.TransactionsExecutor.Services;
using Lykke.Common.Log;
using Newtonsoft.Json;

namespace Lykke.Bil2.Ripple.TransactionsExecutor.Services
{
    public class IntegrationInfoService : IIntegrationInfoService
    {
        private readonly IRippleApi _rippleApi;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILog _log;

        public IntegrationInfoService(IRippleApi rippleApi, IHttpClientFactory httpClientFactory, ILogFactory logFactory)
        {
            _rippleApi = rippleApi;
            _httpClientFactory = httpClientFactory;
            _log = logFactory.CreateLog(this);
        }

        public async Task<IntegrationInfo> GetInfoAsync()
        {
            var serverStateResponse = await _rippleApi.Post(new ServerStateRequest());

            serverStateResponse.Result.ThrowIfError();

            if (serverStateResponse.Result.State.ValidatedLedger == null)
            {
                throw new BlockchainIntegrationException(
                    $"Node didn't return last validated ledger, please, retry. Node state: {serverStateResponse.Result.State.ServerState}");
            }

            Version.TryParse(serverStateResponse.Result.State.BuildVersion.Split('-').First(), out var currentVersion);

            Version latestVersion = null;

            try
            {
                var latestRelease = await _httpClientFactory.CreateClient(Startup.RippleGitHubRepository).GetAsync("releases/latest");

                if (latestRelease.IsSuccessStatusCode)
                {
                    dynamic json = JsonConvert.DeserializeObject(await latestRelease.Content.ReadAsStringAsync());
                    Version.TryParse(((string)json.tag_name).Split('-').First(), out latestVersion);
                }
            }
            catch (Exception ex)
            {
                _log.Warning("Failed to get latest release version", ex);
            }

            return new IntegrationInfo
            (
                new BlockchainInfo
                (
                    serverStateResponse.Result.State.ValidatedLedger.Seq,
                    serverStateResponse.Result.State.ValidatedLedger.CloseTime.FromRippleEpoch()
                ),
                new Dictionary<string, DependencyInfo>
                {
                    ["node"] = new DependencyInfo(currentVersion, latestVersion)
                }
            );
        }
    }
}
