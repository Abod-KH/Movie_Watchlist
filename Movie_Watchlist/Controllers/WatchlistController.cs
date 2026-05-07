using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movie_Watchlist.Application.Helpers;
namespace Movie_Watchlist.Presintation.Controllers
{
    [Authorize]
    public class WatchlistController : Controller
    {
        private readonly IUserWatchlistRepository _watchlistRepo;
        private string _userId => User.GetUserId()!;

        public WatchlistController(IUserWatchlistRepository watchlistRepo)
        {
            _watchlistRepo = watchlistRepo;

        }

        private async Task<WatchlistDashboardViewModel> GetViewModelData(int page = 1)
        {
            int pageSize = 20;
            var (movies, total, watched) = await _watchlistRepo.GetUserWatchlist(_userId, page, pageSize);
            
            var pagedResult = movies.ToPagedResultServer(page, total, pageSize);
            var percentage = total == 0 ? 0 : (int)((double)watched / total * 100);

            ViewBag.CurrentPage = pagedResult.CurrentPage;
            ViewBag.TotalPages = pagedResult.TotalPages;

            return new WatchlistDashboardViewModel
            {
                Movies = pagedResult.Items,
                TotalMovies = total,
                MoviesWatched = watched,
                Percentage = percentage
            };
        }


        public async Task<IActionResult> UserWatchlist(int page = 1)
        {
            var model = await GetViewModelData(page);
            return View(model);
        }


        public async Task<IActionResult> GetWatchlistPartial(int page = 1)
        {
            var model = await GetViewModelData(page);
            return PartialView("_WatchlistContent", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItem(int movieId)
        {

            bool isSuccess = await _watchlistRepo.AddToWatchlist(movieId, _userId);

            if (isSuccess)
            {
                return Ok();
            }
            else
            {

                return BadRequest();
            }
        }
        public async Task<IActionResult> RemoveItem(int movieId)
        {

            await _watchlistRepo.RemoveFromWatchlist(movieId, _userId);
            return RedirectToAction("UserWatchlist");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveItemAjax(int movieId)
        {
            await _watchlistRepo.RemoveFromWatchlist(movieId, _userId);
            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleWatched(int movieId)
        {

            var success = await _watchlistRepo.ToggleWatchedStatus(movieId, _userId);

            if (success) return Ok();
            return BadRequest();
        }
    }
}
