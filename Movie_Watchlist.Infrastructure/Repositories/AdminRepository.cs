using Microsoft.Data.SqlClient;
using Movie_Watchlist.Application.Interfaces;
using Movie_Watchlist.Domain.Entities;
using Movie_Watchlist.Infrastructure.Data;
using System.Data;

namespace Movie_Watchlist.Infrastructure.Repositories
    {
        public class AdminRepository : Repository,IAdminRepository
        {
  
            public AdminRepository(SqlConnectionFactory connectionFactory):base(connectionFactory)
            {
     
               
            }

            public async Task AddMovie(Movie movie)
            {

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
   
                using var connection = _connectionFactory.CreateConnection();
                var command = new SqlCommand("sp_DeleteMovie", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", id);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }


            public async Task<Movie?> GetMovieById(int id)
            {
       
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

