using Microsoft.Data.SqlClient;
using Movie_Watchlist.Application.Interfaces;
using Movie_Watchlist.Domain.Entities;
 
using System.Data;
 

namespace Movie_Watchlist.Infrastructure.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public MovieRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task InsertOrUpdateAsync(IEnumerable<Movie> movies)
        {
            if (movies == null)
                return;

            var table = new DataTable();
            table.Columns.Add("TmdbId", typeof(int));
            table.Columns.Add("Title", typeof(string));
            table.Columns.Add("Description", typeof(string));
            table.Columns.Add("PosterPath", typeof(string));
            table.Columns.Add("ReleaseYear", typeof(int));
            table.Columns.Add("GenreId", typeof(int));

            foreach (var movie in movies)
            {
                table.Rows.Add(
                    movie.TmdbId,
                    movie.Title ?? string.Empty,
                    movie.Description ?? string.Empty,
                    movie.PosterPath ?? string.Empty,
                    movie.ReleaseYear,
                    movie.GenreId
                );
            }

            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("sp_SyncTmdbMovies", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            var param = command.Parameters.AddWithValue("@Movies", table);
            param.SqlDbType = SqlDbType.Structured;
            param.TypeName = "dbo.MovieType";

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }
    }
}
