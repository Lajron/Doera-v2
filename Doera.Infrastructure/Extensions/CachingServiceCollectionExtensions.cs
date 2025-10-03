using Doera.Application.Interfaces.Caching;
using Doera.Infrastructure.Caching;
using Microsoft.Extensions.DependencyInjection;

namespace Doera.Infrastructure.Extensions;

public static class CachingServiceCollectionExtensions
{
    public static IServiceCollection AddAppCaching(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddSingleton<ICacheService, MemoryCacheService>();
        return services;
    }
}