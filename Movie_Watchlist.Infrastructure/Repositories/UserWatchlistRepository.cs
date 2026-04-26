

namespace Movie_Watchlist.Infrastructure.Repositories
{
    public class UserWatchlistRepository : Repository, IUserWatchlistRepository
    {
      
       
        public UserWatchlistRepository(SqlConnectionFactory connectionFactory): base(connectionFactory)
        {
           
        }

        public async Task<bool> AddToWatchlist(int movieId, string userId)
        {
            var result = await ExecuteScalarAsync<int>("sp_AddToWatchlist", 
                ("@MovieId", movieId), ("@UserId", userId));
            return result == 1;
        }

        public async Task<IEnumerable<WatchlistViewModel>> GetUserWatchlist(string userId)
        {
            return await ExecuteQueryListAsync<WatchlistViewModel>("sp_GetUserWatchlist", ("@UserId", userId));
        }

        public async Task<bool> RemoveFromWatchlist(int movieId, string userId)
        {
            var result = await ExecuteScalarAsync<int>("sp_RemoveFromWatchlist", 
                ("@MovieId", movieId), ("@UserId", userId));
            return result > 0;
        }

        public async Task<bool> ToggleWatchedStatus(int movieId, string userId)
        {
            var result = await ExecuteScalarAsync<int>("sp_ToggleWatchlistStatus", 
                ("@MovieId", movieId), ("@UserId", userId));
            return result >= 0;
        }
    }
}
