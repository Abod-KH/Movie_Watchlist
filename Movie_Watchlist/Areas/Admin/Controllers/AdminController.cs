using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Movie_Watchlist.Domain.Entities;



namespace Movie_Watchlist.Presintation.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        
        private readonly IAdminRepository _adminRepo;
        private readonly IFileService _fileService;
        private readonly IHomeRepository _homeRepo; 
        public AdminController(IAdminRepository adminRepo, IFileService fileService, IHomeRepository homeRepo)
        {
            _adminRepo = adminRepo;
            _fileService = fileService;
            _homeRepo = homeRepo;
        }

        [HttpGet]
        public async Task<IActionResult> AddMovie()
        {
            var model = new MovieFormModel
            {
                GenreList = await _homeRepo.Genres()
            };
            return View(model);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
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

            return RedirectToAction("Index", "Home", new { area = "" });
        }

        
        
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var movie = await _adminRepo.GetMovieById(id);
            if (movie == null) return NotFound();
            return View(movie);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Movie movie)
        {
            ModelState.Remove("Genre");

            if (ModelState.IsValid)
            {
                await _adminRepo.UpdateMovie(movie);
                return RedirectToAction("Index", "Home", new { area = "" });
            }
            return View(movie);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _adminRepo.DeleteMovie(id);
            return RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}