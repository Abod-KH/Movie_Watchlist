using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Movie_Watchlist.Models.DTOs;
using Movie_Watchlist.Repositories;
using Movie_Watchlist.Services;
using System.Security.Claims;

namespace Movie_Watchlist.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeRepository _homeRepo;
        
        private readonly IMovieService _movieService;
        private readonly IMemoryCache _cache;
        public HomeController(IHomeRepository homeRepo, IMovieService movieService, IMemoryCache cache)
        {
            _homeRepo = homeRepo;
           
            _movieService = movieService;
            _cache = cache;
        }

        public async Task<IActionResult> Index(string sTerm = "", int genreId = 0, int page = 1)
        {
            const string cacheKey = "LastImportTime";


            if (!_cache.TryGetValue(cacheKey, out DateTime lastImport))
            {
                lastImport = DateTime.MinValue;
            }

            if (DateTime.Now > lastImport.AddMinutes(60))
            {
                await _movieService.ImportMoviesAsync();


                _cache.Set(cacheKey, DateTime.Now);
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var moviesFromRepo = await _homeRepo.GetMoviesForUser(userId, sTerm, genreId);
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
            return View(movie);
        }
    }
}
