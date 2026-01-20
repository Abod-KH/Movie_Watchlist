using Movie_Watchlist.Models;
using Movie_Watchlist.Models.DTOs;

namespace Movie_Watchlist.Repositories
{
    public interface IHomeRepository
    {
        Task<IEnumerable<MovieHomeViewModel>> GetMoviesForUser(string userId, string sTerm = "", int genreId = 0);
        Task<IEnumerable<Genre>> Genres();
        Task<bool> AddToWatchlist(int movieId, string userId);
    }
}
