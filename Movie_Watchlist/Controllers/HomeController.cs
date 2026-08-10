
using Microsoft.AspNetCore.Mvc;

using Microsoft.Extensions.Caching.Memory;
using Movie_Watchlist.Domain.Entities;
using Movie_Watchlist.Application.DTOs;
using System.Security.Claims;



namespace Movie_Watchlist.Presintation.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeRepository _homeRepo;
        private readonly ITmdbService _tmdbService;


        private readonly IHomepageCacheService _cacheService;

        public HomeController(IHomeRepository homeRepo, ITmdbService tmdbService, IHomepageCacheService cacheService)
        {
            _homeRepo = homeRepo;
            _tmdbService = tmdbService;
            _cacheService = cacheService;
        }

        public IActionResult Index()
        {
            var data = _cacheService.GetHomepageData();
            if (data == null)
            {
                // Fallback if cache is empty - worker may not have run yet
                data = new HomepageData();
            }

            var model = new HomepageViewModel
            {
                HeroMovies = data.TrendingMovies?.Take(5).ToList() ?? new List<MovieApiResult>(),
                TrendingMovies = data.TrendingMovies ?? new List<MovieApiResult>(),
                TrendingTvShows = data.TrendingTvShows ?? new List<TvShowApiResult>(),
                NowPlaying = data.NowPlaying ?? new List<MovieApiResult>(),
                Upcoming = data.Upcoming ?? new List<MovieApiResult>(),
                TopRatedMovies = data.TopRatedMovies ?? new List<MovieApiResult>(),
                TopRatedTvShows = data.TopRatedTvShows ?? new List<TvShowApiResult>(),
                PopularMovies = data.PopularMovies ?? new List<MovieApiResult>(),
                PopularTvShows = data.PopularTvShows ?? new List<TvShowApiResult>()
            };

            return View(model);
        }

        public async Task<IActionResult> ShowMore(string category, int page = 1)
        {
            var model = new ShowMoreViewModel
            {
                Category = category,
                CurrentPage = page
            };

            string dbCategory = "";
            string mediaType = "";

            switch (category)
            {
                case "trending-movies":
                    model.SectionTitle = "🔥 Trending Movies"; dbCategory = "trending"; mediaType = "movie"; break;
                case "trending-tv":
                    model.SectionTitle = "🔥 Trending TV Shows"; dbCategory = "trending"; mediaType = "tv"; break;
                case "now-playing":
                    model.SectionTitle = "🎬 Now Playing"; dbCategory = "now_playing"; mediaType = "movie"; break;
                case "upcoming":
                    model.SectionTitle = "📅 Upcoming"; dbCategory = "upcoming"; mediaType = "movie"; break;
                case "top-rated-movies":
                    model.SectionTitle = "⭐ Top Rated Movies"; dbCategory = "top_rated"; mediaType = "movie"; break;
                case "top-rated-tv":
                    model.SectionTitle = "📺 Top Rated TV Shows"; dbCategory = "top_rated"; mediaType = "tv"; break;
                case "popular-movies":
                    model.SectionTitle = "🔥 Popular Movies"; dbCategory = "popular"; mediaType = "movie"; break;
                case "popular-tv":
                    model.SectionTitle = "📺 Popular TV Shows"; dbCategory = "popular"; mediaType = "tv"; break;
                default:
                    return NotFound();
            }

            model.IsMovieSection = mediaType == "movie";
            int pageSize = 20;

            var (items, totalCount) = await _homeRepo.GetCategoryItemsAsync(dbCategory, mediaType, page, pageSize);
            
            model.Items = items.ToList();
            model.TotalItems = totalCount;
            model.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return View(model);
        }

        public async Task<IActionResult> Movies(string sTerm = "", int genreId = 0, int page = 1)
        {
            int pageSize = 20;
            var userId = User.GetUserId();
            var (movies, totalCount) = await _homeRepo.GetMoviesForUser(userId!, sTerm, genreId, page, pageSize);
            var genres = await _homeRepo.Genres();

           

            var model = new MovieDisplayModel
            {
                Movies = movies,
                Filter= new MovieFilterViewModel
                {
                    Genres = genres,
                    STerm = sTerm,
                    GenreId = genreId
                }
            };


            ViewBag.CurrentPage = page;
            ViewBag.TotalItems = totalCount;
            ViewBag.PageSize = pageSize;

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
                TmdbId = apiMovie.Id,
                VoteAverage = apiMovie.VoteAverage
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

        public async Task<IActionResult> Index1()
        {
          

            return View();
        }
        public async Task<IActionResult> Index2()
        {
          

            return View();
        }

    }
}
