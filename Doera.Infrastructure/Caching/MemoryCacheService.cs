using Doera.Application.Interfaces.Caching;
using Microsoft.Extensions.Caching.Memory;

namespace Doera.Infrastructure.Caching {
    internal sealed class MemoryCacheService(IMemoryCache cache) : ICacheService {
        public async Task<T> GetOrCreateAsync<T>(
            string key,
            Func<Task<T>> factory,
            TimeSpan? ttl = null,
            CancellationToken cancellationToken = default) {

            //IMemoryCache has a GetOrCreateAsync ffs...
            if (cache.TryGetValue(key, out T? existing) && existing is not null)
                return existing;

            var created = await factory();
            Set(key, created, ttl);
            return created;
            //return await cache.GetOrCreateAsync(key, async entry =>
            //{
            //    entry.AbsoluteExpirationRelativeToNow = ttl;
            //    return await factory();
            //});
        }

        public bool TryGet<T>(string key, out T? value) {
            if (cache.TryGetValue(key, out var boxed) && boxed is T cast) {
                value = cast;
                return true;
            }
            value = default;
            return false;
        }

        public void Set<T>(string key, T value, TimeSpan? ttl = null) {
            var options = new MemoryCacheEntryOptions();
            if (ttl.HasValue)
                options.AbsoluteExpirationRelativeToNow = ttl;
            cache.Set(key, value, options);
        }

        public void Remove(string key) => cache.Remove(key);
    }
}

    