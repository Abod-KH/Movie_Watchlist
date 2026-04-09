using Microsoft.Data.SqlClient;
using Movie_Watchlist.Application.DTOs;
using Movie_Watchlist.Application.Interfaces;
using Movie_Watchlist.Infrastructure.Data;
using Movie_Watchlist.Infrastructure.Repositories;
using Movie_Watchlist.Application.Models;
using System.Data;


namespace Movie_Watchlist.Repositories
{
    public class UserWatchlistRepository : Repository, IUserWatchlistRepository
    {
      
       
        public UserWatchlistRepository(SqlConnectionFactory connectionFactory): base(connectionFactory)
        {
           
        }

        public async Task<bool> AddToWatchlist(int movieId, string userId)
        {
           
            using var connection = _connectionFactory.CreateConnection();
            var command = new SqlCommand("sp_AddToWatchlist", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@MovieId", movieId);
            command.Parameters.AddWithValue("@UserId", userId);
  

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return result != null && (int)result == 1;
        }

        public async Task<IEnumerable<WatchlistViewModel>> GetUserWatchlist(string userId)
        {
            
            var list = new List<WatchlistViewModel>();
            using var connection = _connectionFactory.CreateConnection();
            var command = new SqlCommand("sp_GetUserWatchlist", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@UserId", userId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(MapReaderToObject<WatchlistViewModel>(reader));
            }
            return list;
        }

        public async Task<bool> RemoveFromWatchlist(int movieId, string userId)
        {
           
            using var connection = _connectionFactory.CreateConnection();
            var command = new SqlCommand("sp_RemoveFromWatchlist", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@MovieId", movieId);
            command.Parameters.AddWithValue("@UserId", userId);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();

            return result != null && Convert.ToInt32(result) > 0;
        }

        public async Task<bool> ToggleWatchedStatus(int movieId, string userId)
        {
           
            using var connection = _connectionFactory.CreateConnection();
            var command = new SqlCommand("sp_ToggleWatchlistStatus", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@MovieId", movieId);
            command.Parameters.AddWithValue("@UserId", userId);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return result != null && Convert.ToInt32(result) == 1;
        }
    }
}
