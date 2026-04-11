using Movie_Watchlist.Application.DTOs;

namespace Movie_Watchlist.Application.Interfaces
{
    public interface ITmdbService
    {
        Task<IEnumerable<MovieApiResult>> GetPopularMoviesAsync();
        
        Task<IEnumerable<MovieApiResult>> SearchMoviesAsync(string query);
    }
}
