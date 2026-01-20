using Microsoft.EntityFrameworkCore;
using Movie_Watchlist.Data;
using Movie_Watchlist.Models;
using Movie_Watchlist.Models.DTOs;

namespace Movie_Watchlist.Repositories
{
    public class HomeRepository : IHomeRepository
    {
        private readonly ApplicationDbContext _db;

        public HomeRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Genre>> Genres()
        {
            return await _db.Genres.ToListAsync();
        }

        public async Task<IEnumerable<MovieHomeViewModel>> GetMoviesForUser(string userId, string sTerm = "", int genreId = 0)
        {
            sTerm = sTerm.ToLower();

            var query = from movie in _db.Movies
                        
                        join genre in _db.Genres on movie.GenreId equals genre.Id

                        join watch in _db.WatchlistItems.Where(w => w.UserId == userId)
                        on movie.Id equals watch.MovieId into wl
                        from watchlist in wl.DefaultIfEmpty()

                        where (string.IsNullOrWhiteSpace(sTerm) || movie.Title.ToLower().Contains(sTerm))
                        where (genreId == 0 || movie.GenreId == genreId)

                        select new MovieHomeViewModel
                        {
                            Movie = movie,
                            IsInWatchlist = watchlist != null,
                            GenreName = genre.Name 
                        };

            return await query.ToListAsync();
        }
        public async Task<bool> AddToWatchlist(int movieId, string userId)
        {
            try
            {
                
                var exists = await _db.WatchlistItems
                    .AnyAsync(w => w.MovieId == movieId && w.UserId == userId);

                if (exists) return false; 

                
                var watchlistItem = new WatchlistItem
                {
                    MovieId = movieId,
                    UserId = userId,
                    DateAdded = DateTime.Now,
                    IsWatched = false
                };

                _db.WatchlistItems.Add(watchlistItem);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}