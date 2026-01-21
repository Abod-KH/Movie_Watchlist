using Movie_Watchlist.Models;
using Movie_Watchlist.Models.DTOs;
using Movie_Watchlist.Data;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Movie_Watchlist.Repositories
{
    public class HomeRepository : IHomeRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;
        //    private readonly ApplicationDbContext _db;

        //     public HomeRepository(ApplicationDbContext db)
        public HomeRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
            // _db = db;
        }

        public async Task<IEnumerable<Genre>> Genres()
        {
            // return await _db.Genres.ToListAsync();
            var genres = new List<Genre>();
            using var connection = _connectionFactory.CreateConnection();
            var command = new SqlCommand("SELECT Id, Name FROM Genre", connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                genres.Add(new Genre
                {
                    Id = (int)reader["Id"],
                    Name = (string)reader["Name"]
                });
            }
            return genres;
        }

        public async Task<IEnumerable<MovieHomeViewModel>> GetMoviesForUser(string userId, string sTerm = "", int genreId = 0)
        {
            //  sTerm = sTerm.ToLower();
            var movies = new List<MovieHomeViewModel>();
            using var connection = _connectionFactory.CreateConnection();
            var command = new SqlCommand("sp_GetMoviesForUser", connection);
            command.CommandType = CommandType.StoredProcedure;
            // var query = from movie in _db.Movies

            // join genre in _db.Genres on movie.GenreId equals genre.Id
            //   join watch in _db.WatchlistItems.Where(w => w.UserId == userId)
            //             on movie.Id equals watch.MovieId into wl
            //             from watchlist in wl.DefaultIfEmpty()
            //             where (string.IsNullOrWhiteSpace(sTerm) || movie.Title.ToLower().Contains(sTerm))
            //             where (genreId == 0 || movie.GenreId == genreId)
            //             select new MovieHomeViewModel
            //             {
            //                 Movie = movie,
            //                 IsInWatchlist = watchlist != null,
            //                 GenreName = genre.Name 
            //             };
            // return await query.ToListAsync();
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@SearchTerm", (object?)sTerm ?? DBNull.Value);
            command.Parameters.AddWithValue("@GenreId", genreId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                movies.Add(new MovieHomeViewModel
                {
                    Movie = new Movie
                    {
                        Id = (int)reader["Id"],
                        Title = (string)reader["Title"],
                        TmdbId = (int)reader["TmdbId"],
                        Description = reader["Description"] as string,
                        PosterPath = reader["PosterPath"] as string,
                        ReleaseYear = (int)reader["ReleaseYear"],
                        GenreId = (int)reader["GenreId"],
                        Genre = new Genre
                        {
                            Id = (int)reader["GenreId"],
                            Name = (string)reader["GenreName"]
                        }
                    },
                    IsInWatchlist = (int)reader["IsInWatchlist"] == 1,
                    GenreName = (string)reader["GenreName"]
                });
            }
            return movies;
        }

        public async Task<bool> AddToWatchlist(int movieId, string userId)
        {
            //    try
            // {
                
            //     var exists = await _db.WatchlistItems
            //         .AnyAsync(w => w.MovieId == movieId && w.UserId == userId);
            //    if (exists) return false; 
            //   var watchlistItem = new WatchlistItem
            //     {
            //         MovieId = movieId,
            //         UserId = userId,
            //         DateAdded = DateTime.Now,
            //         IsWatched = false
            //     };
            //     _db.WatchlistItems.Add(watchlistItem);
            //     await _db.SaveChangesAsync();
            //     return true;
            // }
            // catch (Exception)
            // {
            //     return false;
            // }
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
    }
}