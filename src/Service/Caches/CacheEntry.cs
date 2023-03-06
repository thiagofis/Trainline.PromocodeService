using System;

namespace Trainline.PromocodeService.Service.Caches
{
    public class CacheEntry<TEntity>
    {
        public TEntity Item { get; set; }
        public DateTime Expiry { get; set; }
    }
}
