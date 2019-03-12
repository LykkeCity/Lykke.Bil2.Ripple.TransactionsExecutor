using System;
using System.Net.Http.Headers;
using System.Text;
using JetBrains.Annotations;
using Lykke.Bil2.Ripple.Client;
using Lykke.Bil2.Ripple.TransactionsExecutor.Services;
using Lykke.Bil2.Ripple.TransactionsExecutor.Settings;
using Lykke.Bil2.Sdk.TransactionsExecutor;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Bil2.Ripple.TransactionsExecutor
{
    [UsedImplicitly]
    public class Startup
    {
        public const string IntegrationName = "Ripple";

        [UsedImplicitly]
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return services.BuildBlockchainTransactionsExecutorServiceProvider<AppSettings>(options =>
            {
                options.IntegrationName = IntegrationName;

                // Register required service implementations:

                options.AddressValidatorFactory = ctx =>
                    new AddressValidator(ctx.Services.GetRequiredService<IRippleApi>());

                options.HealthProviderFactory = ctx =>
                    new HealthProvider
                    (
                        /* TODO: Provide specific settings and dependencies, if necessary */
                    );

                options.IntegrationInfoServiceFactory = ctx =>
                    new IntegrationInfoService
                    (
                        /* TODO: Provide specific settings and dependencies, if necessary */
                    );

                options.TransactionBroadcasterFactory = ctx =>
                    new TransactionBroadcaster
                    (
                        /* TODO: Provide specific settings and dependencies, if necessary */
                    );

                options.TransactionsStateProviderFactory = ctx =>
                    new TransactionsStateProvider
                    (
                        /* TODO: Provide specific settings and dependencies, if necessary */
                    );

                options.TransferAmountTransactionsBuilderFactory = ctx =>
                    new TransferAmountTransactionsBuilder
                    (
                        /* TODO: Provide specific settings and dependencies, if necessary */
                    );

                options.TransferAmountTransactionsEstimatorFactory = ctx =>
                    new TransferAmountTransactionsEstimator
                    (
                        /* TODO: Provide specific settings and dependencies, if necessary */
                    );


                // Register additional services

                options.UseSettings = (serviceCollection, settings) =>
                {
                    serviceCollection.AddRippleClient
                    (
                        settings.CurrentValue.NodeUrl,
                        settings.CurrentValue.NodeRpcUsername,
                        settings.CurrentValue.NodeRpcPassword
                    );
                };
            });
        }

        [UsedImplicitly]
        public void Configure(IApplicationBuilder app)
        {
            app.UseBlockchainTransactionsExecutor(options =>
            {
                options.IntegrationName = IntegrationName;
            });
        }
    }
}
