
 
using System.Data;
 

namespace Movie_Watchlist.Infrastructure.Repositories
{
    public class MovieRepository : Repository, IMovieRepository
    {
        public MovieRepository(SqlConnectionFactory connectionFactory) : base(connectionFactory)
        {
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

            await ExecuteTableValueNonQueryAsync("sp_SyncTmdbMovies", "@Movies", table, "dbo.MovieType");

            
        }
    }
}
