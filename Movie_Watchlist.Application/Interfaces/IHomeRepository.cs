using Movie_Watchlist.Application.ViewModels;
using Movie_Watchlist.Domain.Entities;

namespace Movie_Watchlist.Application.Interfaces
{
    public interface IHomeRepository
    {
        Task<(IEnumerable<MovieHomeViewModel> Movies, int TotalCount)> GetMoviesForUser(string userId, string sTerm = "", int genreId = 0, int pageNumber = 1, int pageSize = 20);
        Task<IEnumerable<Genre>> Genres();
        Task<Movie?> GetMovieById(int id);
        Task<Movie?> GetMovieByTmdbId(int tmdbId);
        Task<(IEnumerable<MediaHomeViewModel> Items, int TotalCount)> GetCategoryItemsAsync(string category, string mediaType, int pageNumber = 1, int pageSize = 20);
        Task<(IEnumerable<MediaHomeViewModel> Items, int TotalCount)> SearchMediaAsync(string query, string mediaType, int pageNumber = 1, int pageSize = 20);
        Task UpdateCategoryMappingsAsync(string category, string mediaType, string mappingsJson);
    }
}
