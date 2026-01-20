using Microsoft.EntityFrameworkCore;
using Movie_Watchlist.Data;
using Movie_Watchlist.Models;
using Movie_Watchlist.Models.DTOs;

namespace Movie_Watchlist.Repositories
{
    public class UserWatchlistRepository : IUserWatchlistRepository
    {
        private readonly ApplicationDbContext _db;

        public UserWatchlistRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<bool> AddToWatchlist(int movieId, string userId)
        {
            var exists = await _db.WatchlistItems
                .AnyAsync(w => w.MovieId == movieId && w.UserId == userId);

            if (exists) return false;

            var watchlistItem = new WatchlistItem
            {
                MovieId = movieId,
                UserId = userId,
                DateAdded = DateTime.Now
            };

            _db.WatchlistItems.Add(watchlistItem);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<WatchlistViewModel>> GetUserWatchlist(string userId)
        {
            // This query gets the movies specifically for the logged-in user
            var list = await (from movie in _db.Movies
                              join watch in _db.WatchlistItems on movie.Id equals watch.MovieId
                              where watch.UserId == userId
                              select new WatchlistViewModel
                              {
                                  MovieId = movie.Id,
                                  Title = movie.Title,
                                  PosterPath = movie.PosterPath,
                                  ReleaseYear = movie.ReleaseYear,
                                  IsWatched = watch.IsWatched 
                              }).ToListAsync();

            return list;
        }

        public async Task<bool> RemoveFromWatchlist(int movieId, string userId)
        {
            var item = await _db.WatchlistItems
                .FirstOrDefaultAsync(w => w.MovieId == movieId && w.UserId == userId);

            if (item == null) return false;

            _db.WatchlistItems.Remove(item);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> ToggleWatchedStatus(int movieId, string userId)
        {
            var item = await _db.WatchlistItems
                .FirstOrDefaultAsync(w => w.MovieId == movieId && w.UserId == userId);

            if (item == null) return false;

            item.IsWatched = !item.IsWatched; 
            return await _db.SaveChangesAsync() > 0;
        }
    }
}
