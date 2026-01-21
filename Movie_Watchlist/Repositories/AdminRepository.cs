namespace Movie_Watchlist.Repositories
{
    using global::Movie_Watchlist.Data;
    using global::Movie_Watchlist.Models;
    using Microsoft.Data.SqlClient;
    using System.Data;

    namespace Movie_Watchlist.Repositories
    {
        public class AdminRepository : IAdminRepository
        {
            // private readonly ApplicationDbContext _db;
            private readonly SqlConnectionFactory _connectionFactory;

//public AdminRepository(ApplicationDbContext db)
            public AdminRepository(SqlConnectionFactory connectionFactory)
            {
                //_db = db;
                _connectionFactory = connectionFactory;
            }

            public async Task AddMovie(Movie movie)
            {
                //  _db.Movies.Add(movie);
                // await _db.SaveChangesAsync();
                using var connection = _connectionFactory.CreateConnection();
                var commandText = @"INSERT INTO Movie (Title, TmdbId, Description, PosterPath, ReleaseYear, GenreId) 
                                    VALUES (@Title, @TmdbId, @Description, @PosterPath, @ReleaseYear, @GenreId)";

                var command = new SqlCommand(commandText, connection);
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
                var commandText = @"UPDATE Movie 
                                    SET Title = @Title, TmdbId = @TmdbId, Description = @Description, 
                                        PosterPath = @PosterPath, ReleaseYear = @ReleaseYear, GenreId = @GenreId
                                    WHERE Id = @Id";

                var command = new SqlCommand(commandText, connection);
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
                var commandText = "DELETE FROM Movie WHERE Id = @Id";

                var command = new SqlCommand(commandText, connection);
                command.Parameters.AddWithValue("@Id", id);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }


            public async Task<Movie?> GetMovieById(int id)
            {
                // return await _db.Movies.FindAsync(id);
                using var connection = _connectionFactory.CreateConnection();
                var commandText = "SELECT * FROM Movie WHERE Id = @Id";

                var command = new SqlCommand(commandText, connection);
                command.Parameters.AddWithValue("@Id", id);

                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new Movie
                    {
                        Id = (int)reader["Id"],
                        Title = (string)reader["Title"],
                        TmdbId = (int)reader["TmdbId"],
                        Description = reader["Description"] as string,
                        PosterPath = reader["PosterPath"] as string,
                        ReleaseYear = (int)reader["ReleaseYear"],
                        GenreId = (int)reader["GenreId"]
                    };
                }
                return null;
            }

        }
    }
}
