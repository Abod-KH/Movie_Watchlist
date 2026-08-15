using Movie_Watchlist.Application.Interfaces;
using Movie_Watchlist.Application.ViewModels;
using Movie_Watchlist.Domain.Entities;

namespace Movie_Watchlist.Infrastructure.Repositories
{
    public class TvShowHomeRepository : Repository, ITvShowHomeRepository
    {
        public TvShowHomeRepository(SqlConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public async Task<IEnumerable<Genre>> Genres()
        {
            return await ExecuteQueryListAsync<Genre>("sp_GetAllTvShowGenres");
        }

        public async Task<(IEnumerable<TvShowHomeViewModel> TvShows, int TotalCount)> GetTvShowsForUser(string userId, string sTerm = "", int genreId = 0, int pageNumber = 1, int pageSize = 20)
        {
            var (tvShows, totalCount) = await ExecuteQueryListWithOutputAsync<TvShowHomeViewModel>("sp_GetTvShowsForUser",
                "@TotalCount",
                ("@UserId", userId),
                ("@SearchTerm", sTerm),
                ("@GenreId", genreId),
                ("@PageNumber", pageNumber),
                ("@PageSize", pageSize));

            return (tvShows, totalCount);
        }

        public async Task<TvShow?> GetTvShowById(int id)
        {
            return await ExecuteQuerySingleAsync<TvShow>("sp_GetTvShowDetails", ("@Id", id));
        }

        public async Task<TvShow?> GetTvShowByTmdbId(int tmdbId)
        {
            return await ExecuteQuerySingleAsync<TvShow>("sp_GetTvShowByTmdbId", ("@TmdbId", tmdbId));
        }
    }
}
