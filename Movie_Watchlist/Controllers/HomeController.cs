using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movie_Watchlist.Models.DTOs;
using Movie_Watchlist.Repositories;

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

        public async Task<IActionResult> Index(string sTerm = "", int genreId = 0, int page = 1)
        {
          
            var userId = _userManager.GetUserId(User);

            var moviesFromRepo = await _homeRepo.GetMoviesForUser(userId, sTerm, genreId);
            var genres = await _homeRepo.Genres();

            
            int pageSize = 16;
            int totalMovies = moviesFromRepo.Count();
            int totalPages = (int)Math.Ceiling((double)totalMovies / pageSize);

            var pagedMovies = moviesFromRepo
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

          
            var model = new MovieDisplayModel
            {
                Movies = pagedMovies,
                Genres = genres,
                STerm = sTerm,
                GenreId = genreId
            };

            
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(model);
        }
    }
}
