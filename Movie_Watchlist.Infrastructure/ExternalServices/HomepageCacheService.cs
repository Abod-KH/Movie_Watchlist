using Microsoft.Extensions.Caching.Memory;
using Movie_Watchlist.Application.Interfaces;
using Movie_Watchlist.Application.ViewModels;

namespace Movie_Watchlist.Infrastructure.ExternalServices
{
    public class HomepageCacheService : IHomepageCacheService
    {
        private readonly IMemoryCache _memoryCache;
        private const string CacheKey = "HomepageData_CacheKey";

        public HomepageCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public HomepageData? GetHomepageData()
        {
            _memoryCache.TryGetValue(CacheKey, out HomepageData? data);
            return data;
        }

        public void SetHomepageData(HomepageData data)
        {
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromHours(25)); // slightly more than 24h cycle
            
            _memoryCache.Set(CacheKey, data, cacheOptions);
        }
    }
}
