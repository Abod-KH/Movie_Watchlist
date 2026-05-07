


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
            var movies = await ExecuteQueryListAsync<MovieHomeViewModel>("sp_GetMoviesForUser",
                ("@UserId", userId),
                ("@SearchTerm", sTerm),
                ("@GenreId", genreId),
                ("@PageNumber", pageNumber),
                ("@PageSize", pageSize));

            var count = await ExecuteScalarAsync<int>("sp_GetMoviesForUserCount",
                ("@SearchTerm", sTerm),
                ("@GenreId", genreId));

            return (movies, count);
        }

        public async Task<Movie?> GetMovieById(int id)
        {
            return await ExecuteQuerySingleAsync<Movie>("sp_GetMovieDetails", ("@Id", id));
        }

        public async Task<Movie?> GetMovieByTmdbId(int tmdbId)
        {
            return await ExecuteQuerySingleAsync<Movie>("sp_GetMovieByTmdbId", ("@TmdbId", tmdbId));
        }
    }
}