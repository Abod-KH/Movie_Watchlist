using Movie_Watchlist.Application.DTOs;

namespace Movie_Watchlist.Application.Interfaces
{
    public interface ITmdbService
    {
        Task<IEnumerable<int>> GetChangedMovieIdsAsync();
        
        Task<MovieApiResult?> GetMovieDetailsAsync(int tmdbId);
        Task<string?> GetMovieTrailerKeyAsync(int tmdbId);
        Task<IEnumerable<MovieApiResult>> GetSimilarMoviesAsync(int tmdbId);
    }
}
