using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movie_Watchlist.Application.Interfaces;
using Movie_Watchlist.Application.ViewModels;
using System.Security.Claims;

namespace Movie_Watchlist.Controllers
{
    [Authorize]
    public class TvShowController : Controller
    {
        private readonly ITvShowHomeRepository _tvShowHomeRepo;
        private readonly ITmdbService _tmdbService;

        public TvShowController(ITvShowHomeRepository tvShowHomeRepo, ITmdbService tmdbService)
        {
            _tvShowHomeRepo = tvShowHomeRepo;
            _tmdbService = tmdbService;
        }

        public async Task<IActionResult> Index(string sTerm = "", int genreId = 0, int page = 1)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            if (page < 1) page = 1;
            int pageSize = 20;

            var (tvShows, totalCount) = await _tvShowHomeRepo.GetTvShowsForUser(userId, sTerm, genreId, page, pageSize);
            var genres = await _tvShowHomeRepo.Genres();

            var model = new TvShowDisplayModel
            {
                TvShows = tvShows,
                Genres = genres,
                STerm = sTerm,
                GenreId = genreId,
                PageNumber = page,
                TotalItems = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
ViewBag.CurrentPage = page;
            ViewBag.TotalItems = totalCount;
            ViewBag.PageSize = pageSize;
            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var tvShow = await _tvShowHomeRepo.GetTvShowById(id);
            if (tvShow == null) return NotFound();

            var similarShows = await _tmdbService.GetSimilarTvShowsAsync(tvShow.TmdbId);
            var trailerKey = await _tmdbService.GetTvShowTrailerKeyAsync(tvShow.TmdbId);

            var model = new TvShowDetailsViewModel
            {
                TvShow = tvShow,
                TrailerKey = trailerKey,
                SimilarTvShows = similarShows.ToList()
            };

            return View(model);
        }

        public async Task<IActionResult> DetailsByTmdb(int tmdbId)
        {
            var tvShow = await _tvShowHomeRepo.GetTvShowByTmdbId(tmdbId);
            if (tvShow != null)
            {
                return RedirectToAction(nameof(Details), new { id = tvShow.Id });
            }

            var apiTvShow = await _tmdbService.GetTvShowDetailsAsync(tmdbId);
            if (apiTvShow == null) return NotFound();

            var similarShows = await _tmdbService.GetSimilarTvShowsAsync(tmdbId);
            var trailerKey = await _tmdbService.GetTvShowTrailerKeyAsync(tmdbId);

            int releaseYear = 0;
            if (DateTime.TryParse(apiTvShow.FirstAirDate, out var date))
            {
                releaseYear = date.Year;
            }

            var model = new TvShowDetailsViewModel
            {
                TvShow = new Domain.Entities.TvShow
                {
                    TmdbId = apiTvShow.Id,
                    Title = apiTvShow.Title ?? "Unknown",
                    Description = apiTvShow.Description ?? "",
                    PosterPath = apiTvShow.FullPosterPath,
                    Rating = apiTvShow.Rating,
                    ReleaseYear = releaseYear
                },
                TrailerKey = trailerKey,
                SimilarTvShows = similarShows.ToList()
            };

            return View("Details", model);
        }
    }
}
