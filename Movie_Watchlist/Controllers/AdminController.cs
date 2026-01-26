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
       

        public AdminController(IAdminRepository adminRepo, IFileService fileService, IHomeRepository homeRepo, ITmdbService tmdbService)
        {
            _adminRepo = adminRepo;
            _fileService = fileService;
            _homeRepo = homeRepo;
            _tmdbService = tmdbService;
           
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

        
        
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var movie = await _adminRepo.GetMovieById(id);
            if (movie == null) return NotFound();
            return View(movie);
        }

        
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

        
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _adminRepo.DeleteMovie(id);
            return RedirectToAction("Index", "Home");
        }
    }
}