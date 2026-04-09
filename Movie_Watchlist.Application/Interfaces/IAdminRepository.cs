using Movie_Watchlist.Domain.Entities;

namespace Movie_Watchlist.Application.Interfaces
{
    public interface IAdminRepository
    {
        Task AddMovie(Movie movie);
        Task UpdateMovie(Movie movie);
        Task DeleteMovie(int id);
        Task<Movie?> GetMovieById(int id);
    }
}
