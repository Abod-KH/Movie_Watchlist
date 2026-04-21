


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

        public async Task<IEnumerable<MovieHomeViewModel>> GetMoviesForUser(string userId, string sTerm = "", int genreId = 0)
        {
            return await ExecuteQueryListAsync<MovieHomeViewModel>("sp_GetMoviesForUser",
                ("@UserId", userId),
                ("@SearchTerm", sTerm),
                ("@GenreId", genreId));
        }

        public async Task<Movie?> GetMovieById(int id)
        {
            return await ExecuteQuerySingleAsync<Movie>("sp_GetMovieDetails", ("@Id", id));
        }


    }
}