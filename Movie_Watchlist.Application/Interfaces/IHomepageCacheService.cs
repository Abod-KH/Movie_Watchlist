using Movie_Watchlist.Application.ViewModels;

namespace Movie_Watchlist.Application.Interfaces
{
    public interface IHomepageCacheService
    {
        HomepageData? GetHomepageData();
        void SetHomepageData(HomepageData data);
        Task<HomepageData> LoadHomepageFromDatabaseAsync(CancellationToken cancellationToken = default);
    }
}
