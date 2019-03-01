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
                    new AddressValidator
                    (
                        /* TODO: Provide specific settings and dependencies, if necessary */
                    );

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

                options.TransactionEstimatorFactory = ctx =>
                    new TransactionEstimator
                    (
                        /* TODO: Provide specific settings and dependencies, if necessary */
                    );

                options.TransactionExecutorFactory = ctx =>
                    new TransactionExecutor
                    (
                        /* TODO: Provide specific settings and dependencies, if necessary */
                    );

                // Register additional services

                options.UseSettings = settings =>
                {
                    services.AddRippleClient
                    (
                        settings.CurrentValue.NodeUrl,
                        settings.CurrentValue.NodeRpcUsername,
                        settings.CurrentValue.NodeRpcPassword
                    );

                    //services.AddSingleton<IService>(new ServiceImpl(settings.CurrentValue.ServiceSettingValue));
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
