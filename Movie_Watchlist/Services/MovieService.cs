using Movie_Watchlist.Data;
using Movie_Watchlist.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Movie_Watchlist.Services
{


    public class MovieService : IMovieService
    {
        private readonly ITmdbService _tmdbService;
        private readonly SqlConnectionFactory _connectionFactory;
//    private readonly ApplicationDbContext _db;    
//    public MovieService(ITmdbService tmdbService, ApplicationDbContext db)
        public MovieService(ITmdbService tmdbService, SqlConnectionFactory connectionFactory)
        {
            _tmdbService = tmdbService;
            _connectionFactory = connectionFactory;
            // _db = db;
        }

        public async Task ImportMoviesAsync()
        {
            var apiMovies = await _tmdbService.GetPopularMoviesAsync();

            var table = new DataTable();
            table.Columns.Add("TmdbId", typeof(int));
            table.Columns.Add("Title", typeof(string));
            table.Columns.Add("Description", typeof(string));
            table.Columns.Add("PosterPath", typeof(string));
            table.Columns.Add("ReleaseYear", typeof(int));
            table.Columns.Add("GenreId", typeof(int));

            foreach (var apiMovie in apiMovies)
            {
                //  if (!_db.Movies.Any(m => m.TmdbId == apiMovie.Id))
                // {
                //     int genreToUse = (apiMovie.Genre_ids != null && apiMovie.Genre_ids.Any())
                //          ? apiMovie.Genre_ids.First()
                //          : 28;
                //     var movie = new Movie
                //     {
                //         TmdbId = apiMovie.Id,
                //         Title = apiMovie.Title,
                //         ReleaseYear = int.TryParse(apiMovie.ReleaseDate?.Split('-')[0], out int y) ? y : 0,
                //         GenreId = genreToUse,
                //         Description = apiMovie.Description,
                //         PosterPath = apiMovie.FullPosterPath
                //     };
                //     _db.Movies.Add(movie);
                // }
                int genreToUse = (apiMovie.Genre_ids != null && apiMovie.Genre_ids.Any())
                        ? apiMovie.Genre_ids.First()
                        : 28;

                table.Rows.Add(
                   apiMovie.Id,
                   apiMovie.Title,
                   apiMovie.Description,
                   apiMovie.FullPosterPath,
                   int.TryParse(apiMovie.ReleaseDate?.Split('-')[0], out int y) ? y : 0,
                   genreToUse
               );
            }
            // await _db.SaveChangesAsync();
            using var connection = _connectionFactory.CreateConnection();
            var command = new SqlCommand("sp_SyncTmdbMovies", connection);
            command.CommandType = CommandType.StoredProcedure;

            var param = command.Parameters.AddWithValue("@Movies", table);
            param.SqlDbType = SqlDbType.Structured;
            param.TypeName = "dbo.MovieType";

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }
    }
}
