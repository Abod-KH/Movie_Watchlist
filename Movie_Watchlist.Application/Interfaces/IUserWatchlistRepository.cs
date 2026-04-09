using Movie_Watchlist.Application.DTOs;


namespace Movie_Watchlist.Application.Interfaces
{
    public interface IUserWatchlistRepository
    {
        Task<bool> AddToWatchlist(int movieId, string userId);
        Task<bool> RemoveFromWatchlist(int movieId, string userId);
        Task<IEnumerable<WatchlistViewModel>> GetUserWatchlist(string userId);
        Task<bool> ToggleWatchedStatus(int movieId, string userId);
    }
}
