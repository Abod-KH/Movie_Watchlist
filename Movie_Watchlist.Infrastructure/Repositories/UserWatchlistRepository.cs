
namespace Movie_Watchlist.Infrastructure.Repositories
{
    public class UserWatchlistRepository : Repository, IUserWatchlistRepository
    {
        public UserWatchlistRepository(SqlConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public async Task<bool> AddToWatchlist(int movieId, string userId)
        {
            var result = await ExecuteScalarAsync<int>("sp_AddToWatchlist",
                ("@MovieId", movieId), ("@UserId", userId));
            return result == 1;
        }

        public async Task<(IEnumerable<WatchlistViewModel> Movies, int TotalCount, int WatchedCount)>
            GetUserWatchlist(string userId, int pageNumber = 1, int pageSize = 20)
        {
            var movies = await ExecuteQueryListAsync<WatchlistViewModel>(
                "sp_GetUserWatchlist",
                ("@UserId", userId),
                ("@PageNumber", pageNumber),
                ("@PageSize", pageSize));

            var firstMovie = movies.FirstOrDefault();

            int totalCount = firstMovie?.TotalCount ?? 0;
            int watchedCount = firstMovie?.WatchedCount ?? 0;

            return (movies, totalCount, watchedCount);
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

        public async Task<bool> IsInWatchlistAsync(int movieId, string userId)
        {
            const string sql = @"
                SELECT CASE WHEN EXISTS (
                    SELECT 1 FROM dbo.WatchlistItem
                    WHERE MovieId = @MovieId AND UserId = @UserId
                ) THEN 1 ELSE 0 END";
            return await ExecuteTextScalarAsync(sql,
                ("@MovieId", movieId),
                ("@UserId", userId)) == 1;
        }

        public async Task<bool> ToggleInWatchlistAsync(int movieId, string userId)
        {
            const string sql = @"
                DECLARE @Exists BIT;
                SELECT @Exists = CASE WHEN EXISTS (
                    SELECT 1 FROM dbo.WatchlistItem WHERE MovieId = @MovieId AND UserId = @UserId
                ) THEN 1 ELSE 0 END;

                IF @Exists = 1
                BEGIN
                    DELETE FROM dbo.WatchlistItem WHERE MovieId = @MovieId AND UserId = @UserId;
                    SELECT 0;
                END
                ELSE
                BEGIN
                    INSERT INTO dbo.WatchlistItem (UserId, MovieId, DateAdded, IsWatched)
                    VALUES (@UserId, @MovieId, GETDATE(), 0);
                    SELECT 1;
                END";
            return await ExecuteTextScalarAsync(sql,
                ("@MovieId", movieId),
                ("@UserId", userId)) == 1;
        }
    }
}
