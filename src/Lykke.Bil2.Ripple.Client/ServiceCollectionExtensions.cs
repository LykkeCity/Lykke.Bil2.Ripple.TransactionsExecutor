using System;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace Lykke.Bil2.Ripple.Client
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the JSON RPC client of the Ripple blockchain to the app services as <see cref="IRippleApi"/>.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="url">Base URL of node.</param>
        /// <param name="username">Username, if authentication required for JSON RPC.</param>
        /// <param name="password">Password, if authentication required for JSON RPC.</param>
        /// <returns></returns>
        public static IServiceCollection AddRippleClient(this IServiceCollection services, string url, string username, string password)
        {
            if (string.IsNullOrWhiteSpace(url) || !Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out _))
            {
                throw new ArgumentException("Invalid node URL", nameof(url));
            }

            services.AddRefitClient<IRippleApi>()
                .ConfigureHttpClient((serviceProvider, client) =>
                {
                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Authorization = !string.IsNullOrEmpty(username)
                        ? new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}")))
                        : null;
                });

            return services;
        }
    }
}