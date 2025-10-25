
namespace Doera.Application.Interfaces.Caching {
    public interface ICacheService {
        Task<T> GetOrCreateAsync<T>(
            string key,
            Func<Task<T>> factory,
            TimeSpan? ttl = null,
            CancellationToken cancellationToken = default
        );

        bool TryGet<T>(string key, out T? value);
        void Set<T>(string key, T value, TimeSpan? ttl = null);
        void Remove(string key);
    }
}
