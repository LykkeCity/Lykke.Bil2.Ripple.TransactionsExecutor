using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.TransactionsExecutor.Responses;
using Lykke.Bil2.Sdk.TransactionsExecutor.Models;
using Lykke.Bil2.Sdk.TransactionsExecutor.Services;

namespace Lykke.Bil2.Ripple.TransactionsExecutor.Services
{
    public class IntegrationInfoService : IIntegrationInfoService
    {
        public IntegrationInfoService(/* TODO: Provide specific settings and dependencies, if necessary */)
        {
        }

        public async Task<IntegrationInfo> GetInfoAsync()
        {
            // TODO: Provide current blockchain state (last confirmed block number and time).
            //
            // If possible, return current and available new version (if any) of all components, of which current service is dependent.
            // For Example:
            //
            // var info = ...;
            // var nodeAvailableVersion = ...;
            //
            // return new IntegrationInfo
            // (
            //     new BlockchainInfo(info.LastIrreversibleBlockNumber, info.LastIrreversibleBlockTime),
            //     new Dictionary<string, DependencyInfo>
            //     {
            //         { "node", new DependencyInfo(new Version(info.CurrentVersion), new Version(nodeAvailableVersion)) }
            //     }
            // );


            throw new System.NotImplementedException();
        }
    }
}
