namespace Movie_Watchlist.Repositories
{
    using global::Movie_Watchlist.Data;
    using global::Movie_Watchlist.Models;


    namespace Movie_Watchlist.Repositories
    {
        public class AdminRepository : IAdminRepository
        {
            private readonly ApplicationDbContext _db;

            public AdminRepository(ApplicationDbContext db)
            {
                _db = db;
            }

            public async Task AddMovie(Movie movie)
            {
                _db.Movies.Add(movie);
                await _db.SaveChangesAsync();
            }

            // We will implement Update/Delete/Get later as we need them!
            public async Task<Movie> GetMovieById(int id) => throw new NotImplementedException();
            public async Task UpdateMovie(Movie movie) => throw new NotImplementedException();
            public async Task DeleteMovie(int id) => throw new NotImplementedException();
        }
    }
}
