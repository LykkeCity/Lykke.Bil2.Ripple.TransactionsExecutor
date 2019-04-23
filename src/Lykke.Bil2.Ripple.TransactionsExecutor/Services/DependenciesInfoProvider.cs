using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.TransactionsExecutor.Responses;
using Lykke.Bil2.Ripple.Client;
using Lykke.Bil2.Ripple.Client.Api.ServerState;
using Lykke.Bil2.Sdk.TransactionsExecutor.Services;
using Lykke.Bil2.SharedDomain;
using Newtonsoft.Json;

namespace Lykke.Bil2.Ripple.TransactionsExecutor.Services
{
    public class DependenciesInfoProvider : IDependenciesInfoProvider
    {
        private readonly IRippleApi _rippleApi;
        private readonly IHttpClientFactory _httpClientFactory;

        public DependenciesInfoProvider(IRippleApi rippleApi, IHttpClientFactory httpClientFactory)
        {
            _rippleApi = rippleApi;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IReadOnlyDictionary<DependencyName, DependencyInfo>> GetInfoAsync()
        {
            var serverStateResponse = await _rippleApi.Post(new ServerStateRequest());

            serverStateResponse.Result.ThrowIfError();

            var latestRelease = await _httpClientFactory.CreateClient(Startup.RippleGitHubRepository).GetAsync("releases/latest");

            latestRelease.EnsureSuccessStatusCode();

            dynamic json = JsonConvert.DeserializeObject(await latestRelease.Content.ReadAsStringAsync());

            return new Dictionary<DependencyName, DependencyInfo>
            {
                ["node"] = new DependencyInfo(serverStateResponse.Result.State.BuildVersion, (string)json.tag_name)
            };
        }
    }
}
