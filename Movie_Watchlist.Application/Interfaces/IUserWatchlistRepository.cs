using Movie_Watchlist.Application.ViewModels;


namespace Movie_Watchlist.Application.Interfaces
{
    public interface IUserWatchlistRepository
    {
        Task<bool> AddToWatchlist(int movieId, string userId);
        Task<bool> RemoveFromWatchlist(int movieId, string userId);
        Task<(IEnumerable<WatchlistViewModel> Movies, int TotalCount, int WatchedCount)> GetUserWatchlist(string userId, int pageNumber = 1, int pageSize = 20);
        Task<bool> ToggleWatchedStatus(int movieId, string userId);
    }
}
