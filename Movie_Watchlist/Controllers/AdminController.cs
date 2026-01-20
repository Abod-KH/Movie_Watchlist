using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movie_Watchlist.Data;
using Movie_Watchlist.Models;
using Movie_Watchlist.Models.DTOs;
using Movie_Watchlist.Repositories;
using Movie_Watchlist.Services;

namespace Movie_Watchlist.Controllers
{
    [Authorize(Roles = "Admin")] 
    public class AdminController : Controller
    {
        
        private readonly IAdminRepository _adminRepo;
        private readonly IFileService _fileService;
        private readonly IHomeRepository _homeRepo; 
        private readonly ITmdbService _tmdbService;
        private ApplicationDbContext _db;

        public AdminController(IAdminRepository adminRepo, IFileService fileService, IHomeRepository homeRepo, ITmdbService tmdbService, ApplicationDbContext db)
        {
            _adminRepo = adminRepo;
            _fileService = fileService;
            _homeRepo = homeRepo;
            _tmdbService = tmdbService;
            _db = db;
        }

      
        public async Task<IActionResult> AddMovie()
        {
            var model = new MovieFormModel
            {
                GenreList = await _homeRepo.Genres()
            };
            return View(model);
        }

       
        [HttpPost]
        public async Task<IActionResult> AddMovie(MovieFormModel model)
        {
            if (!ModelState.IsValid)
            {
                
                model.GenreList = await _homeRepo.Genres();
                return View(model);
            }

            
            string imageFileName = "";
            if (model.ImageFile != null)
            {
                imageFileName = await _fileService.SaveImage(model.ImageFile);
            }

           
            var movie = new Movie
            {
                Title = model.Title,
                ReleaseYear = model.ReleaseYear,
                GenreId = model.GenreId,
                PosterPath = "/images/" + imageFileName 
            };

            
            await _adminRepo.AddMovie(movie);

            return RedirectToAction("Index", "Home");
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BulkImport()
        {
            var apiMovies = await _tmdbService.GetPopularMoviesAsync();
            int count = 0;

            foreach (var apiMovie in apiMovies)
            {
                // Check if movie already exists in your SQL database by Title
                bool exists = _db.Movies.Any(m => m.Title == apiMovie.Title);

                if (!exists)
                {
                    int year = 0;
                    if (!string.IsNullOrEmpty(apiMovie.ReleaseDate))
                    {
                        int.TryParse(apiMovie.ReleaseDate.Split('-')[0], out year);
                    }

                    var movie = new Movie
                    {
                        Title = apiMovie.Title,
                        ReleaseYear = year,
                        GenreId = apiMovie.Genre_ids?.FirstOrDefault() ?? 28,
                        Description = apiMovie.Description,
                        PosterPath = apiMovie.FullPosterPath
                    };

                    _db.Movies.Add(movie);
                    count++;
                }
            }

            await _db.SaveChangesAsync(); 
            TempData["Message"] = $"Imported {count} new movies!";
            return RedirectToAction("Index", "Home");
        }
        // 1. GET: Show the Edit Form
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var movie = await _adminRepo.GetMovieById(id);
            if (movie == null) return NotFound();
            return View(movie);
        }

        // 2. POST: Process the Edit
        [HttpPost]
        public async Task<IActionResult> Edit(Movie movie)
        {
            ModelState.Remove("Genre");

            if (ModelState.IsValid)
            {
                await _adminRepo.UpdateMovie(movie);
                return RedirectToAction("Index", "Home");
            }
            return View(movie);
        }

        // 3. POST: Process the Delete
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _adminRepo.DeleteMovie(id);
            return RedirectToAction("Index", "Home");
        }
    }
}