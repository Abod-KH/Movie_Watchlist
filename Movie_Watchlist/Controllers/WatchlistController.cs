using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Movie_Watchlist.Application.Interfaces;
using Movie_Watchlist.Application.Models;
using System.Security.Claims;
namespace Movie_Watchlist.Presintation.Controllers
{
    [Authorize] 
    public class WatchlistController : Controller
    {
        private readonly IUserWatchlistRepository _watchlistRepo; 
        

        public WatchlistController(IUserWatchlistRepository watchlistRepo)
        {
            _watchlistRepo = watchlistRepo;
            
        }

        private async Task<WatchlistDashboardViewModel> GetViewModelData()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var movies = await _watchlistRepo.GetUserWatchlist(userId);

            var total = movies.Count();
            var watched = movies.Count(m => m.IsWatched);
            var percentage = total == 0 ? 0 : (int)((double)watched / total * 100);

            return new WatchlistDashboardViewModel
            {
                Movies = movies,
                TotalMovies = total,
                MoviesWatched = watched,
                Percentage = percentage
            };
        }

    
        public async Task<IActionResult> UserWatchlist()
        {
            var model = await GetViewModelData();
            return View(model);
        }

        
        public async Task<IActionResult> GetWatchlistPartial()
        {
            var model = await GetViewModelData(); 
            return PartialView("_WatchlistContent", model);
        }
        [HttpPost]
        public async Task<IActionResult> AddItem(int movieId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _watchlistRepo.AddToWatchlist(movieId, userId);
            return Ok();
        }
        public async Task<IActionResult> RemoveItem(int movieId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _watchlistRepo.RemoveFromWatchlist(movieId, userId);
            return RedirectToAction("UserWatchlist");
        }
        
        [HttpPost]
        public async Task<IActionResult> ToggleWatched(int movieId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var success = await _watchlistRepo.ToggleWatchedStatus(movieId, userId);

            if (success) return Ok();
            return BadRequest();
        }
    }
}
