using Microsoft.AspNetCore.Mvc;
using Movie_Watchlist.Application.Interfaces;
using Movie_Watchlist.Application.ViewModels;

namespace Movie_Watchlist.Controllers
{
    public class SearchController : Controller
    {
        private readonly IHomeRepository _homeRepository;

        public SearchController(IHomeRepository homeRepository)
        {
            _homeRepository = homeRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string q, string type = "all", int page = 1)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return View(new SearchViewModel());
            }

            int pageSize = 20;
            var (items, totalCount) = await _homeRepository.SearchMediaAsync(q, type, page, pageSize);

            ViewBag.SearchTerm = q;
            ViewBag.SearchType = type;

            var model = new SearchViewModel
            {
                Items = items.ToList(),
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalCount
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Autocomplete(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return Json(new List<object>());
            }

            var (items, _) = await _homeRepository.SearchMediaAsync(q, "all", 1, 10);
            
            var results = items.Select(i => new
            {
                id = i.Id,
                title = i.Title,
                mediaType = i.MediaType,
                releaseYear = i.ReleaseYear,
                posterPath = i.PosterPath
            });

            return Json(results);
        }
    }
}
