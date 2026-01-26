using Movie_Watchlist.Models;
using Movie_Watchlist.Models.DTOs;
using Movie_Watchlist.Data;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Movie_Watchlist.Repositories
{
    public class HomeRepository : Repository, IHomeRepository
    {
        
        //    private readonly ApplicationDbContext _db;

        //     public HomeRepository(ApplicationDbContext db)
        public HomeRepository(SqlConnectionFactory connectionFactory):base(connectionFactory)
        {
            
            // _db = db;
        }

        public async Task<IEnumerable<Genre>> Genres()
        {
            // return await _db.Genres.ToListAsync();
            var genres = new List<Genre>();
            using var connection = _connectionFactory.CreateConnection();
            var command = new SqlCommand("sp_GetAllGenres", connection);
            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                genres.Add(MapReaderToObject<Genre>(reader));
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
            command.Parameters.AddWithValue("@UserId", (object?)userId ?? DBNull.Value);
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

        public async Task<Movie?> GetMovieById(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            var command = new SqlCommand("sp_GetMovieDetails", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapReaderToObject<Movie>(reader) : null;
        }


    }
}