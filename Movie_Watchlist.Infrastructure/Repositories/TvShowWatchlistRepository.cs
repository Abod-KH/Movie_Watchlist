using Movie_Watchlist.Application.Interfaces;
using Movie_Watchlist.Application.ViewModels;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Movie_Watchlist.Infrastructure.Repositories
{
    public class TvShowWatchlistRepository : Repository, ITvShowWatchlistRepository
    {
        public TvShowWatchlistRepository(SqlConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public async Task<bool> AddToWatchlist(int tvShowId, string userId)
        {
            try
            {
                await ExecuteNonQueryAsync("sp_AddTvShowToWatchlist", ("@TvShowId", tvShowId), ("@UserId", userId));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<(IEnumerable<TvShowWatchlistViewModel> TvShows, int TotalCount, int WatchedCount)> GetUserWatchlist(string userId, int pageNumber = 1, int pageSize = 20)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("sp_GetUserTvShowWatchlist", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 60;

            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@PageNumber", pageNumber);
            command.Parameters.AddWithValue("@PageSize", pageSize);

            var totalCountParam = new SqlParameter("@TotalCount", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var watchedCountParam = new SqlParameter("@WatchedCount", SqlDbType.Int) { Direction = ParameterDirection.Output };
            
            command.Parameters.Add(totalCountParam);
            command.Parameters.Add(watchedCountParam);

            var list = new List<TvShowWatchlistViewModel>();
            await connection.OpenAsync();
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    list.Add(MapReaderToObject<TvShowWatchlistViewModel>(reader));
                }
            }

            int totalCount = totalCountParam.Value != DBNull.Value ? (int)totalCountParam.Value : 0;
            int watchedCount = watchedCountParam.Value != DBNull.Value ? (int)watchedCountParam.Value : 0;

            return (list, totalCount, watchedCount);
        }

        public async Task<bool> RemoveFromWatchlist(int tvShowId, string userId)
        {
            try
            {
                await ExecuteNonQueryAsync("sp_RemoveTvShowFromWatchlist", ("@TvShowId", tvShowId), ("@UserId", userId));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ToggleWatchedStatus(int tvShowId, string userId)
        {
            try
            {
                await ExecuteNonQueryAsync("sp_ToggleTvShowWatchedStatus", ("@TvShowId", tvShowId), ("@UserId", userId));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
