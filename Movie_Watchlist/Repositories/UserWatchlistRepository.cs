using Movie_Watchlist.Data;
using Movie_Watchlist.Models;
using Movie_Watchlist.Models.DTOs;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Movie_Watchlist.Repositories
{
    public class UserWatchlistRepository : IUserWatchlistRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;
        //  private readonly ApplicationDbContext _db;
     
        // public UserWatchlistRepository(ApplicationDbContext db)
        public UserWatchlistRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
            //  _db = db;
        }

        public async Task<bool> AddToWatchlist(int movieId, string userId)
        {
            //  var exists = await _db.WatchlistItems
            //     .AnyAsync(w => w.MovieId == movieId && w.UserId == userId);
            //  if (exists) return false;
            //  var watchlistItem = new WatchlistItem
            // {
            //     MovieId = movieId,
            //     UserId = userId,
            //     DateAdded = DateTime.Now
            // };
            // _db.WatchlistItems.Add(watchlistItem);
            // return await _db.SaveChangesAsync() > 0;
            using var connection = _connectionFactory.CreateConnection();
            var commandText = @"
                IF NOT EXISTS (SELECT 1 FROM WatchlistItem WHERE MovieId = @MovieId AND UserId = @UserId)
                BEGIN
                    INSERT INTO WatchlistItem (MovieId, UserId, DateAdded, IsWatched)
                    VALUES (@MovieId, @UserId, @DateAdded, 0);
                    SELECT 1;
                END
                ELSE
                BEGIN
                    SELECT 0;
                END";

            var command = new SqlCommand(commandText, connection);
            command.Parameters.AddWithValue("@MovieId", movieId);
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@DateAdded", DateTime.Now);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return result != null && (int)result == 1;
        }

        public async Task<IEnumerable<WatchlistViewModel>> GetUserWatchlist(string userId)
        {
             // This query gets the movies specifically for the logged-in user
            // var list = await (from movie in _db.Movies
            //                   join watch in _db.WatchlistItems on movie.Id equals watch.MovieId
            //                   where watch.UserId == userId
            //                   select new WatchlistViewModel
            //                   {
            //                       MovieId = movie.Id,
            //                       Title = movie.Title,
            //                       PosterPath = movie.PosterPath,
            //                       ReleaseYear = movie.ReleaseYear,
            //                       IsWatched = watch.IsWatched 
            //                   }).ToListAsync();
            var list = new List<WatchlistViewModel>();
            using var connection = _connectionFactory.CreateConnection();
            var command = new SqlCommand("sp_GetUserWatchlist", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@UserId", userId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new WatchlistViewModel
                {
                    MovieId = (int)reader["MovieId"],
                    Title = (string)reader["Title"],
                    PosterPath = reader["PosterPath"] as string,
                    ReleaseYear = (int)reader["ReleaseYear"],
                    IsWatched = (bool)reader["IsWatched"]
                });
            }
            return list;
        }

        public async Task<bool> RemoveFromWatchlist(int movieId, string userId)
        {
            // var item = await _db.WatchlistItems
            //     .FirstOrDefaultAsync(w => w.MovieId == movieId && w.UserId == userId);
            
              // if (item == null) return false;

            // _db.WatchlistItems.Remove(item);
            // return await _db.SaveChangesAsync() > 0;
            using var connection = _connectionFactory.CreateConnection();
            var commandText = "DELETE FROM WatchlistItem WHERE MovieId = @MovieId AND UserId = @UserId";

            var command = new SqlCommand(commandText, connection);
            command.Parameters.AddWithValue("@MovieId", movieId);
            command.Parameters.AddWithValue("@UserId", userId);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> ToggleWatchedStatus(int movieId, string userId)
        {
            // var item = await _db.WatchlistItems
            //     .FirstOrDefaultAsync(w => w.MovieId == movieId && w.UserId == userId);
            //   if (item == null) return false;
            
            // item.IsWatched = !item.IsWatched; 
            // return await _db.SaveChangesAsync() > 0;
            using var connection = _connectionFactory.CreateConnection();
            var commandText = @"UPDATE WatchlistItem 
                                SET IsWatched = CASE WHEN IsWatched = 1 THEN 0 ELSE 1 END
                                WHERE MovieId = @MovieId AND UserId = @UserId";

            var command = new SqlCommand(commandText, connection);
            command.Parameters.AddWithValue("@MovieId", movieId);
            command.Parameters.AddWithValue("@UserId", userId);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
    }
}
