 
using Microsoft.AspNetCore.Mvc;
 
using Microsoft.Extensions.Caching.Memory;
 
using System.Security.Claims;
 

namespace Movie_Watchlist.Presintation.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeRepository _homeRepo;
        private readonly Movie_Watchlist.Application.Interfaces.ITmdbService _tmdbService;
        
       
        public HomeController(IHomeRepository homeRepo, Movie_Watchlist.Application.Interfaces.ITmdbService tmdbService)
        {
            _homeRepo = homeRepo;
            _tmdbService = tmdbService;
           
        }
       
        public async Task<IActionResult> Index(string sTerm = "", int genreId = 0, int page = 1)
        {
            
            var  userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var moviesFromRepo = await _homeRepo.GetMoviesForUser(userId! , sTerm, genreId);
            var genres = await _homeRepo.Genres();


            int pageSize = 20;
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

        public async Task<IActionResult> Details(int id)
        {
            var movie = await _homeRepo.GetMovieById(id);
            if (movie == null)
            {
                return NotFound();
            }
            
            if (movie.TmdbId > 0)
            {
                ViewBag.TrailerKey = await _tmdbService.GetMovieTrailerKeyAsync(movie.TmdbId);
            }
            
            return View(movie);
        }

       


    }
}
