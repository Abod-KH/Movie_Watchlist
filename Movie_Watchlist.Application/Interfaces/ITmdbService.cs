using Movie_Watchlist.Application.DTOs;

namespace Movie_Watchlist.Application.Interfaces
{
    public interface ITmdbService
    {
        Task<IEnumerable<int>> GetChangedMovieIdsAsync();
        
        Task<MovieApiResult?> GetMovieDetailsAsync(int tmdbId);
        Task<string?> GetMovieTrailerKeyAsync(int tmdbId);
        Task<IEnumerable<MovieApiResult>> GetSimilarMoviesAsync(int tmdbId);
        
        // TV Show methods
        Task<IEnumerable<int>> GetChangedTvShowIdsAsync();
        Task<TvShowApiResult?> GetTvShowDetailsAsync(int tmdbId);
        Task<string?> GetTvShowTrailerKeyAsync(int tmdbId);
        Task<IEnumerable<TvShowApiResult>> GetSimilarTvShowsAsync(int tmdbId);

        // Homepage list endpoints (paginated)
        Task<TmdbPagedResponse<MovieApiResult>?> GetTrendingMoviesAsync(int page = 1);
        Task<TmdbPagedResponse<TvShowApiResult>?> GetTrendingTvShowsAsync(int page = 1);
        Task<TmdbPagedResponse<MovieApiResult>?> GetNowPlayingMoviesAsync(int page = 1);
        Task<TmdbPagedResponse<MovieApiResult>?> GetUpcomingMoviesAsync(int page = 1);
        Task<TmdbPagedResponse<MovieApiResult>?> GetTopRatedMoviesAsync(int page = 1);
        Task<TmdbPagedResponse<TvShowApiResult>?> GetTopRatedTvShowsAsync(int page = 1);
        Task<TmdbPagedResponse<MovieApiResult>?> GetPopularMoviesAsync(int page = 1);
        Task<TmdbPagedResponse<TvShowApiResult>?> GetPopularTvShowsAsync(int page = 1);
    }
}
