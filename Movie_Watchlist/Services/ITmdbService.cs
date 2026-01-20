using Movie_Watchlist.Models;

namespace Movie_Watchlist.Services
{
    public interface ITmdbService
    {
        Task<IEnumerable<MovieApiResult>> GetPopularMoviesAsync();
        
        Task<IEnumerable<MovieApiResult>> SearchMoviesAsync(string query);
    }
}
