using Microsoft.Extensions.DependencyInjection;
using System;

namespace ExtraLife
{
    public static class ServiceCollectionExtensions
    {
        public static void AddExtraLifeClient(this IServiceCollection services)
        {
            services.AddScoped<IExtraLifeApiClient, ExtraLifeApiClient>()
                .AddHttpClient<IExtraLifeApiClient, ExtraLifeApiClient>(client =>
                {
                    client.BaseAddress = new Uri("https://www.extra-life.org/api/");
                });
        }
    }
}
