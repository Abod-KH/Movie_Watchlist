using Movie_Watchlist.Application.ViewModels;

namespace Movie_Watchlist.Application.Interfaces
{
    public interface ITvShowWatchlistRepository
    {
        Task<bool> AddToWatchlist(int tvShowId, string userId);
        Task<bool> RemoveFromWatchlist(int tvShowId, string userId);
        Task<(IEnumerable<TvShowWatchlistViewModel> TvShows, int TotalCount, int WatchedCount)> GetUserWatchlist(
            string userId, int pageNumber = 1, int pageSize = 20);
        Task<bool> ToggleWatchedStatus(int tvShowId, string userId);
        Task<bool> IsInWatchlistAsync(int tvShowId, string userId);
        Task<bool> ToggleInWatchlistAsync(int tvShowId, string userId);
    }
}
