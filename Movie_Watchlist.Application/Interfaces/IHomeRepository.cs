using Movie_Watchlist.Application.ViewModels;
using Movie_Watchlist.Domain.Entities;

namespace Movie_Watchlist.Application.Interfaces
{
    public interface IHomeRepository
    {
        Task<IEnumerable<MovieHomeViewModel>> GetMoviesForUser(string userId, string sTerm = "", int genreId = 0);
        Task<IEnumerable<Genre>> Genres();
        Task<Movie?> GetMovieById(int id);
        Task<Movie?> GetMovieByTmdbId(int tmdbId);
    }
}
