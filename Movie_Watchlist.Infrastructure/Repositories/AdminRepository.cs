

namespace Movie_Watchlist.Infrastructure.Repositories
    {
        public class AdminRepository : Repository,IAdminRepository
        {
  
            public AdminRepository(SqlConnectionFactory connectionFactory):base(connectionFactory)
            {
     
               
            }

            public async Task AddMovie(Movie movie)
            {
                await ExecuteNonQueryAsync("sp_InsertMovie", 
                    ("@Title", movie.Title),
                    ("@TmdbId", movie.TmdbId),
                    ("@Description", movie.Description),
                    ("@PosterPath", movie.PosterPath),
                    ("@ReleaseYear", movie.ReleaseYear),
                    ("@GenreId", movie.GenreId));
            }

            public async Task UpdateMovie(Movie movie)
            {
                await ExecuteNonQueryAsync("sp_UpdateMovie", 
                    ("@Id", movie.Id),
                    ("@Title", movie.Title),
                    ("@TmdbId", movie.TmdbId),
                    ("@Description", movie.Description),
                    ("@PosterPath", movie.PosterPath),
                    ("@ReleaseYear", movie.ReleaseYear),
                    ("@GenreId", movie.GenreId));
            }

            public async Task DeleteMovie(int id)
            {
                await ExecuteNonQueryAsync("sp_DeleteMovie", ("@Id", id));
            }


            public async Task<Movie?> GetMovieById(int id)
            {
                return await ExecuteQuerySingleAsync<Movie>("sp_GetMovieById", ("@Id", id));
            }

        }
    }

