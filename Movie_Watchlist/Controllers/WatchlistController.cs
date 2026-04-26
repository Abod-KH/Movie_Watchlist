using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        private async Task<WatchlistDashboardViewModel> GetViewModelData()
        {
            
            var movies = await _watchlistRepo.GetUserWatchlist(_userId);

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
        public async Task<IActionResult> RemoveItemAjax(int movieId)
        {
            await _watchlistRepo.RemoveFromWatchlist(movieId, _userId);
            return Ok();
        }
        
        [HttpPost]
        public async Task<IActionResult> ToggleWatched(int movieId)
        {
            
            var success = await _watchlistRepo.ToggleWatchedStatus(movieId, _userId);

            if (success) return Ok();
            return BadRequest();
        }
    }
}
