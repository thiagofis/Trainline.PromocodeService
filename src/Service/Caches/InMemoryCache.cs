using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Trainline.PromocodeService.Service.Caches
{
    public class InMemoryCache<T>
    {
        private readonly ConcurrentDictionary<string, CacheEntry<T>> _innerCache = new ConcurrentDictionary<string, CacheEntry<T>>();
        private readonly IDateTimeProvider _dateTimeProvider;

        public InMemoryCache(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<T> GetOrAdd(string key, Func<Task<T>> getSelector)
        {
            if (_innerCache.TryGetValue(key, out var cachedData))
            {
                if (!HasExpired(cachedData))
                {
                    return cachedData.Item;
                }
                _innerCache.TryRemove(key, out _);
            }

            var data = await getSelector();
            _innerCache.TryAdd(key, new CacheEntry<T>
            {
                Item = data,
                Expiry = GetClosestExpiryDate()
            });

            return data;
        }

        private DateTime GetClosestExpiryDate()
        {
            var now = _dateTimeProvider.UtcNow;
            return new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0)
                .AddHours(1);
        }

        private bool HasExpired(CacheEntry<T> entry)
        {
            return entry.Expiry <= _dateTimeProvider.UtcNow;
        }
    }
}
