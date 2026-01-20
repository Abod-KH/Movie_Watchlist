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

            public async Task UpdateMovie(Movie movie)
            {
                
                _db.Movies.Update(movie);
                await _db.SaveChangesAsync();
            }

            public async Task DeleteMovie(int id)
            {
               
                var movie = await _db.Movies.FindAsync(id);
                if (movie != null)
                {
                    _db.Movies.Remove(movie);
                    await _db.SaveChangesAsync();
                }
            }


            public async Task<Movie> GetMovieById(int id)
            {
                // FindAsync is the most efficient way to get a single row by ID
                // It will return null if the movie is not found.
                return await _db.Movies.FindAsync(id);
            }

        }
    }
}
