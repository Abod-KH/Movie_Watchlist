using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movie_Watchlist.Application.Interfaces;
using Movie_Watchlist.Application.ViewModels;
using System.Security.Claims;

namespace Movie_Watchlist.Controllers
{
    [Authorize]
    public class TvShowWatchlistController : Controller
    {
        private readonly ITvShowWatchlistRepository _watchlistRepo;

        public TvShowWatchlistController(ITvShowWatchlistRepository watchlistRepo)
        {
            _watchlistRepo = watchlistRepo;
        }

        public async Task<IActionResult> UserWatchlist(int page = 1)
        {
            if (page < 1) page = 1;
            int pageSize = 20;

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var (tvShows, totalCount, watchedCount) = await _watchlistRepo.GetUserWatchlist(userId, page, pageSize);

            var model = new TvShowWatchlistDashboardViewModel
            {
                TvShows = tvShows,
                TotalCount = totalCount,
                WatchedCount = watchedCount,
                PageNumber = page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetWatchlistPartial(int page = 1)
        {
            if (page < 1) page = 1;
            int pageSize = 20;

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var (tvShows, totalCount, watchedCount) = await _watchlistRepo.GetUserWatchlist(userId, page, pageSize);

            var model = new TvShowWatchlistDashboardViewModel
            {
                TvShows = tvShows,
                TotalCount = totalCount,
                WatchedCount = watchedCount,
                PageNumber = page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return PartialView("_WatchlistContent", model);
        }

        [HttpPost]
        public async Task<IActionResult> AddItem(int tvShowId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            bool isAdded = await _watchlistRepo.AddToWatchlist(tvShowId, userId);

            if (isAdded) return Ok("Added to Watchlist");
            return BadRequest("Could not add to Watchlist");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveItem(int tvShowId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            bool isRemoved = await _watchlistRepo.RemoveFromWatchlist(tvShowId, userId);

             if (!isRemoved)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(UserWatchlist));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromWatchlist(int id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            bool isRemoved = await _watchlistRepo.RemoveFromWatchlist(id, userId);

            if (!isRemoved) return NotFound();
            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int tvShowId)
        {
            if (tvShowId <= 0) return BadRequest(new { isInWatchlist = false });
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            bool isInWatchlist = await _watchlistRepo.ToggleInWatchlistAsync(tvShowId, userId);
            return Ok(new { isInWatchlist });
        }
    }
}
