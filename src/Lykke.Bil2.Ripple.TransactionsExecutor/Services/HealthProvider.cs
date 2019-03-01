using System.Threading.Tasks;
using Lykke.Bil2.Sdk.TransactionsExecutor.Services;

namespace Lykke.Bil2.Ripple.TransactionsExecutor.Services
{
    public class HealthProvider : IHealthProvider
    {
        public HealthProvider(/* TODO: Provide specific settings and dependencies, if necessary */)
        {
        }

        public async Task<string> GetDiseaseAsync()
        {
            // TODO: check state of node and/or service and return description of problems, if any.
            //
            // It is a good place to check if node is synchronized, if possible.
            // For example:
            //
            // try
            // {
            //     if (!await _nodeClient.GetIsSynchronizedAsync())
            //     {
            //         return "Node is not synchronized";
            //     }
            // }
            // catch (HttpRequestException ex)
            // {
            //     return $"Node is unavailable: {ex}";
            // }


            throw new System.NotImplementedException();
        }
    }
}
