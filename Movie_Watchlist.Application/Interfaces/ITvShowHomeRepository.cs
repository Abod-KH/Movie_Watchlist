using Movie_Watchlist.Application.ViewModels;
using Movie_Watchlist.Domain.Entities;

namespace Movie_Watchlist.Application.Interfaces
{
    public interface ITvShowHomeRepository
    {
        Task<(IEnumerable<TvShowHomeViewModel> TvShows, int TotalCount)> GetTvShowsForUser(
            string userId, string sTerm = "", int genreId = 0, int pageNumber = 1, int pageSize = 20);
        Task<IEnumerable<Genre>> Genres();
        Task<TvShow?> GetTvShowById(int id);
        Task<TvShow?> GetTvShowByTmdbId(int tmdbId);
    }
}
