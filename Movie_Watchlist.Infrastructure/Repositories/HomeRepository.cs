using Microsoft.Data.SqlClient;
using System.Data;
using Movie_Watchlist.Application.Interfaces;
using Movie_Watchlist.Domain.Entities;
using Movie_Watchlist.Application.DTOs;
using Movie_Watchlist.Infrastructure.Data;

namespace Movie_Watchlist.Infrastructure.Repositories
{
    public class HomeRepository : Repository, IHomeRepository
    {
        

        public HomeRepository(SqlConnectionFactory connectionFactory):base(connectionFactory)
        {
            
   
        }

        public async Task<IEnumerable<Genre>> Genres()
        {
        
            var genres = new List<Genre>();
            using var connection = _connectionFactory.CreateConnection();
            var command = new SqlCommand("sp_GetAllGenres", connection);
            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                genres.Add(MapReaderToObject<Genre>(reader));
            }
            return genres;
        }

        public async Task<IEnumerable<MovieHomeViewModel>> GetMoviesForUser(string userId, string sTerm = "", int genreId = 0)
        {
            
            var movies = new List<MovieHomeViewModel>();
            using var connection = _connectionFactory.CreateConnection();
            var command = new SqlCommand("sp_GetMoviesForUser", connection);
            command.CommandType = CommandType.StoredProcedure;
            
            command.Parameters.AddWithValue("@UserId", (object?)userId ?? DBNull.Value);
            command.Parameters.AddWithValue("@SearchTerm", (object?)sTerm ?? DBNull.Value);
            command.Parameters.AddWithValue("@GenreId", genreId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var movie = MapReaderToObject<Movie>(reader);
                movie.Genre = new Genre
                {
                    Id = (int)reader["GenreId"],
                    Name = (string)reader["GenreName"]
                };

                movies.Add(new MovieHomeViewModel
                {
                    Movie = movie,
                    IsInWatchlist = (int)reader["IsInWatchlist"] == 1,
                    GenreName = movie.Genre.Name
                });
            }
            return movies;
        }

        public async Task<Movie?> GetMovieById(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            var command = new SqlCommand("sp_GetMovieDetails", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapReaderToObject<Movie>(reader) : null;
        }


    }
}