using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Movie_Watchlist.Models;
using Movie_Watchlist.Repositories;
namespace Movie_Watchlist.Controllers
{
    [Authorize] 
    public class WatchlistController : Controller
    {
        private readonly IUserWatchlistRepository _watchlistRepo; 
        private readonly UserManager<IdentityUser> _userManager;

        public WatchlistController(IUserWatchlistRepository watchlistRepo, UserManager<IdentityUser> userManager)
        {
            _watchlistRepo = watchlistRepo;
            _userManager = userManager;
        }

        public async Task<IActionResult> UserWatchlist()
        {
            var userId = _userManager.GetUserId(User);
            var movies = await _watchlistRepo.GetUserWatchlist(userId);

            var total = movies.Count();
            var watched = movies.Count(m => m.IsWatched);
            var percentage = total == 0 ? 0 : (int)((double)watched / total * 100);

            
            var model = new WatchlistDashboardViewModel
            {
                Movies = movies,
                TotalMovies = total,
                MoviesWatched = watched,
                Percentage = percentage
            };

            return View(model);
        }

        public async Task<IActionResult> AddItem(int movieId)
        {
            var userId = _userManager.GetUserId(User);
            await _watchlistRepo.AddToWatchlist(movieId, userId);
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> RemoveItem(int movieId)
        {
            var userId = _userManager.GetUserId(User);
            await _watchlistRepo.RemoveFromWatchlist(movieId, userId);
            return RedirectToAction("UserWatchlist");
        }
        [HttpPost]
        public async Task<IActionResult> ToggleWatched(int movieId)
        {
            var userId = _userManager.GetUserId(User);
            var success = await _watchlistRepo.ToggleWatchedStatus(movieId, userId);

            if (success) return Ok();
            return BadRequest();
        }
    }
}
