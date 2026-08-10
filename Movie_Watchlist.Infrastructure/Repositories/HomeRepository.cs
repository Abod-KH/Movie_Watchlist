


namespace Movie_Watchlist.Infrastructure.Repositories
{
    public class HomeRepository : Repository, IHomeRepository
    {


        public HomeRepository(SqlConnectionFactory connectionFactory) : base(connectionFactory)
        {


        }

        public async Task<IEnumerable<Genre>> Genres()
        {
            return await ExecuteQueryListAsync<Genre>("sp_GetAllGenres");
        }

        public async Task<(IEnumerable<MovieHomeViewModel> Movies, int TotalCount)> GetMoviesForUser(string userId, string sTerm = "", int genreId = 0, int pageNumber = 1, int pageSize = 20)
        {
            var (movies, totalCount) = await ExecuteQueryListWithOutputAsync<MovieHomeViewModel>("sp_GetMoviesForUser",
                "@TotalCount",
                ("@UserId", userId),
                ("@SearchTerm", sTerm),
                ("@GenreId", genreId),
                ("@PageNumber", pageNumber),
                ("@PageSize", pageSize));

            return (movies, totalCount);
        }

        public async Task<Movie?> GetMovieById(int id)
        {
            return await ExecuteQuerySingleAsync<Movie>("sp_GetMovieDetails", ("@Id", id));
        }

        public async Task<Movie?> GetMovieByTmdbId(int tmdbId)
        {
            return await ExecuteQuerySingleAsync<Movie>("sp_GetMovieByTmdbId", ("@TmdbId", tmdbId));
        }

        public async Task<(IEnumerable<MediaHomeViewModel> Items, int TotalCount)> GetCategoryItemsAsync(string category, string mediaType, int pageNumber = 1, int pageSize = 20)
        {
            return await ExecuteQueryListWithOutputAsync<MediaHomeViewModel>("sp_GetCategoryItems",
                "@TotalCount",
                ("@Category", category),
                ("@MediaType", mediaType),
                ("@Page", pageNumber),
                ("@PageSize", pageSize));
        }

        public async Task<(IEnumerable<MediaHomeViewModel> Items, int TotalCount)> SearchMediaAsync(string query, string mediaType, int pageNumber = 1, int pageSize = 20)
        {
            return await ExecuteQueryListWithOutputAsync<MediaHomeViewModel>("sp_SearchMedia",
                "@TotalCount",
                ("@SearchTerm", query),
                ("@MediaType", mediaType),
                ("@Page", pageNumber),
                ("@PageSize", pageSize));
        }

        public async Task UpdateCategoryMappingsAsync(string category, string mediaType, string mappingsJson)
        {
            await ExecuteNonQueryAsync("sp_UpdateCategoryMappings",
                ("@Category", category),
                ("@MediaType", mediaType),
                ("@MappingsJson", mappingsJson));
        }
    }
}