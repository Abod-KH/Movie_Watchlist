using Movie_Watchlist.Models;
using Movie_Watchlist.Models.DTOs;

namespace Movie_Watchlist.Repositories
{
    public interface IUserWatchlistRepository
    {
        Task<bool> AddToWatchlist(int movieId, string userId);
        Task<bool> RemoveFromWatchlist(int movieId, string userId);
        Task<IEnumerable<WatchlistViewModel>> GetUserWatchlist(string userId);
        Task<bool> ToggleWatchedStatus(int movieId, string userId);
    }
}
