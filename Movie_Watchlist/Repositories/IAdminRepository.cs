using Movie_Watchlist.Models;

namespace Movie_Watchlist.Repositories
{
    public interface IAdminRepository
    {
        Task AddMovie(Movie movie);
        Task UpdateMovie(Movie movie);
        Task DeleteMovie(int id);
        Task<Movie?> GetMovieById(int id);
    }
}
