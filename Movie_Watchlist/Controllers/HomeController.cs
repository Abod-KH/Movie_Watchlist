using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Movie_Watchlist.Models;
using Movie_Watchlist.Models.DTOs;
using Movie_Watchlist.Repositories;
using System.Diagnostics;

namespace Movie_Watchlist.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeRepository _homeRepo;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(IHomeRepository homeRepo, UserManager<IdentityUser> userManager)
        {
            _homeRepo = homeRepo;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index(string sTerm = "", int genreId = 0)
        {
            var userId = _userManager.GetUserId(User);
            var movies = await _homeRepo.GetMoviesForUser(userId, sTerm, genreId);
            var genres = await _homeRepo.Genres();

            var model = new MovieDisplayModel
            {
                Movies = movies,
                Genres = genres
            };
            return View(model);
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
