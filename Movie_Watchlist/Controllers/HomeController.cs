
using Microsoft.AspNetCore.Mvc;

using Microsoft.Extensions.Caching.Memory;
using Movie_Watchlist.Domain.Entities;
using System.Security.Claims;
using Movie_Watchlist.Application.Helpers;


namespace Movie_Watchlist.Presintation.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeRepository _homeRepo;
        private readonly ITmdbService _tmdbService;


        public HomeController(IHomeRepository homeRepo, ITmdbService tmdbService)
        {
            _homeRepo = homeRepo;
            _tmdbService = tmdbService;

        }

        public async Task<IActionResult> Index(string sTerm = "", int genreId = 0, int page = 1)
        {
            int pageSize = 20;
            var userId = User.GetUserId();
            var (movies, totalCount) = await _homeRepo.GetMoviesForUser(userId!, sTerm, genreId, page, pageSize);
            var genres = await _homeRepo.Genres();

            var pagedResult = movies.ToPagedResultServer(page, totalCount, pageSize);

            var model = new MovieDisplayModel
            {
                Movies = pagedResult.Items,
                Genres = genres,
                STerm = sTerm,
                GenreId = genreId
            };


            ViewBag.CurrentPage = pagedResult.CurrentPage;
            ViewBag.TotalPages = pagedResult.TotalPages;

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var movie = await _homeRepo.GetMovieById(id);

            if (movie == null)
            {
                return NotFound();
            }
            var model = await BuildModelAsync(movie);

            return View(model);
        }

        public async Task<IActionResult> DetailsByTmdb(int tmdbId)
        {
            var movie = await _homeRepo.GetMovieByTmdbId(tmdbId);

            if (movie != null)
            {
                var model = await BuildModelAsync(movie);
                return View("Details", model);
            }

            var apiMovie = await _tmdbService.GetMovieDetailsAsync(tmdbId);
            if (apiMovie == null)
                return NotFound();

            var mappedMovie = new Movie
            {
                Title = apiMovie.Title ?? "Unknown",
                Description = apiMovie.Description ?? string.Empty,
                PosterPath = apiMovie.FullPosterPath,
                TmdbId = apiMovie.Id
            };

            var modelFromApi = await BuildModelAsync(mappedMovie);

            return View("Details", modelFromApi);
        }

        private async Task<MovieDetailsViewModel> BuildModelAsync(Movie movie)
        {
            var trailerKey = await _tmdbService.GetMovieTrailerKeyAsync(movie.TmdbId);
            var similarMovies = await _tmdbService.GetSimilarMoviesAsync(movie.TmdbId);

            return new MovieDetailsViewModel
            {
                Movie = movie,
                TrailerKey = trailerKey,
                SimilarMovies = similarMovies
            };
        }


    }
}
