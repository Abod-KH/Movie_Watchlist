namespace Movie_Watchlist.Repositories
{
    using global::Movie_Watchlist.Data;
    using global::Movie_Watchlist.Models;
    using Microsoft.Data.SqlClient;
    using System.Data;

    namespace Movie_Watchlist.Repositories
    {
        public class AdminRepository : Repository,IAdminRepository
        {
            // private readonly ApplicationDbContext _db;
            

            //public AdminRepository(ApplicationDbContext db)
            public AdminRepository(SqlConnectionFactory connectionFactory):base(connectionFactory)
            {
                //_db = db;
               
            }

            public async Task AddMovie(Movie movie)
            {
                //  _db.Movies.Add(movie);
                // await _db.SaveChangesAsync();
                using var connection = _connectionFactory.CreateConnection();
                var command = new SqlCommand("sp_InsertMovie", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@Title", movie.Title);
                command.Parameters.AddWithValue("@TmdbId", movie.TmdbId);
                command.Parameters.AddWithValue("@Description", (object?)movie.Description ?? DBNull.Value);
                command.Parameters.AddWithValue("@PosterPath", (object?)movie.PosterPath ?? DBNull.Value);
                command.Parameters.AddWithValue("@ReleaseYear", movie.ReleaseYear);
                command.Parameters.AddWithValue("@GenreId", movie.GenreId);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }

            public async Task UpdateMovie(Movie movie)
            {
                // _db.Movies.Update(movie);
                // await _db.SaveChangesAsync();
                using var connection = _connectionFactory.CreateConnection();
                var command = new SqlCommand("sp_UpdateMovie", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", movie.Id);
                command.Parameters.AddWithValue("@Title", movie.Title);
                command.Parameters.AddWithValue("@TmdbId", movie.TmdbId);
                command.Parameters.AddWithValue("@Description", (object?)movie.Description ?? DBNull.Value);
                command.Parameters.AddWithValue("@PosterPath", (object?)movie.PosterPath ?? DBNull.Value);
                command.Parameters.AddWithValue("@ReleaseYear", movie.ReleaseYear);
                command.Parameters.AddWithValue("@GenreId", movie.GenreId);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }

            public async Task DeleteMovie(int id)
            {
                //     var movie = await _db.Movies.FindAsync(id);
                // if (movie != null)
                // {
                //     _db.Movies.Remove(movie);
                //     await _db.SaveChangesAsync();
                // }
                using var connection = _connectionFactory.CreateConnection();
                var command = new SqlCommand("sp_DeleteMovie", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", id);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }


            public async Task<Movie?> GetMovieById(int id)
            {
                // return await _db.Movies.FindAsync(id);
                using var connection = _connectionFactory.CreateConnection();
                var command = new SqlCommand("sp_GetMovieById", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", id);

                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                return await reader.ReadAsync() ? MapReaderToObject<Movie>(reader) : null;
            }

        }
    }
}
