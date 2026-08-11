using System.Data;
using Movie_Watchlist.Application.Interfaces;
using Movie_Watchlist.Domain.Entities;

namespace Movie_Watchlist.Infrastructure.Repositories
{
    public class TvShowRepository : Repository, ITvShowRepository
    {
        public TvShowRepository(SqlConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public async Task InsertOrUpdateAsync(IEnumerable<TvShow> tvShows)
        {
            if (tvShows == null || !tvShows.Any())
                return;

            var table = new DataTable();
            table.Columns.Add("TmdbId", typeof(int));
            table.Columns.Add("Title", typeof(string));
            table.Columns.Add("Description", typeof(string));
            table.Columns.Add("PosterPath", typeof(string));
            table.Columns.Add("BackdropPath", typeof(string));
            table.Columns.Add("Rating", typeof(double));
            table.Columns.Add("ReleaseYear", typeof(int));
            table.Columns.Add("GenreId", typeof(int));

            foreach (var show in tvShows)
            {
                table.Rows.Add(
                    show.TmdbId,
                    show.Title ?? string.Empty,
                    show.Description ?? string.Empty,
                    show.PosterPath ?? string.Empty,
                    show.BackdropPath ?? string.Empty,
                    show.Rating,
                    show.ReleaseYear,
                    show.GenreId
                );
            }

            await ExecuteTableValueNonQueryAsync("sp_SyncTmdbTvShows", "@TvShows", table, "dbo.TvShowType");
        }
    }
}
