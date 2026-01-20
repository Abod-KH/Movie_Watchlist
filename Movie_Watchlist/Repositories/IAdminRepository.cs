using Movie_Watchlist.Models;

namespace Movie_Watchlist.Repositories
{
    public interface IAdminRepository
    {
        Task AddMovie(Movie movie);
        Task<Movie> GetMovieById(int id);
        Task UpdateMovie(Movie movie);
        Task DeleteMovie(int id);
    }
}
