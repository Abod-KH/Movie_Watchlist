
using Microsoft.AspNetCore.Mvc;

using Microsoft.Extensions.Caching.Memory;
using Movie_Watchlist.Domain.Entities;
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
                // Fallback if cache is empty
                data = new HomepageData();
            }

            var model = new HomepageViewModel
            {
                HeroMovies = data.TrendingMovies.Take(5).ToList(), // Top 5 for rotating hero
                TrendingMovies = data.TrendingMovies,
                TrendingTvShows = data.TrendingTvShows,
                NowPlaying = data.NowPlaying,
                Upcoming = data.Upcoming,
                TopRatedMovies = data.TopRatedMovies,
                TopRatedTvShows = data.TopRatedTvShows,
                PopularMovies = data.PopularMovies,
                PopularTvShows = data.PopularTvShows
            };

            return View(model);
        }

        public async Task<IActionResult> ShowMore(string category, int page = 1)
        {
            var model = new ShowMoreViewModel
            {
                Category = category,
                CurrentPage = page,
                TotalPages = 500 // TMDB usually limits to 500 pages
            };

            switch (category)
            {
                case "trending-movies":
                    model.SectionTitle = "🔥 Trending Movies";
                    model.IsMovieSection = true;
                    var tmRes = await _tmdbService.GetTrendingMoviesAsync(page);
                    if (tmRes != null) { model.Movies = tmRes.Results; model.TotalPages = tmRes.TotalPages; }
                    break;
                case "trending-tv":
                    model.SectionTitle = "🔥 Trending TV Shows";
                    model.IsMovieSection = false;
                    var ttRes = await _tmdbService.GetTrendingTvShowsAsync(page);
                    if (ttRes != null) { model.TvShows = ttRes.Results; model.TotalPages = ttRes.TotalPages; }
                    break;
                case "now-playing":
                    model.SectionTitle = "🎬 Now Playing";
                    model.IsMovieSection = true;
                    var npRes = await _tmdbService.GetNowPlayingMoviesAsync(page);
                    if (npRes != null) { model.Movies = npRes.Results; model.TotalPages = npRes.TotalPages; }
                    break;
                case "upcoming":
                    model.SectionTitle = "📅 Upcoming";
                    model.IsMovieSection = true;
                    var uRes = await _tmdbService.GetUpcomingMoviesAsync(page);
                    if (uRes != null) { model.Movies = uRes.Results; model.TotalPages = uRes.TotalPages; }
                    break;
                case "top-rated-movies":
                    model.SectionTitle = "⭐ Top Rated Movies";
                    model.IsMovieSection = true;
                    var trmRes = await _tmdbService.GetTopRatedMoviesAsync(page);
                    if (trmRes != null) { model.Movies = trmRes.Results; model.TotalPages = trmRes.TotalPages; }
                    break;
                case "top-rated-tv":
                    model.SectionTitle = "📺 Top Rated TV Shows";
                    model.IsMovieSection = false;
                    var trtRes = await _tmdbService.GetTopRatedTvShowsAsync(page);
                    if (trtRes != null) { model.TvShows = trtRes.Results; model.TotalPages = trtRes.TotalPages; }
                    break;
                case "popular-movies":
                    model.SectionTitle = "🔥 Popular Movies";
                    model.IsMovieSection = true;
                    var pmRes = await _tmdbService.GetPopularMoviesAsync(page);
                    if (pmRes != null) { model.Movies = pmRes.Results; model.TotalPages = pmRes.TotalPages; }
                    break;
                case "popular-tv":
                    model.SectionTitle = "📺 Popular TV Shows";
                    model.IsMovieSection = false;
                    var ptRes = await _tmdbService.GetPopularTvShowsAsync(page);
                    if (ptRes != null) { model.TvShows = ptRes.Results; model.TotalPages = ptRes.TotalPages; }
                    break;
                default:
                    return NotFound();
            }

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
