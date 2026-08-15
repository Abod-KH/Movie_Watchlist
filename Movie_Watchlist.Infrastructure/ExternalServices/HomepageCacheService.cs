using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Movie_Watchlist.Application.DTOs;
using Movie_Watchlist.Application.Interfaces;
using Movie_Watchlist.Application.ViewModels;

namespace Movie_Watchlist.Infrastructure.ExternalServices
{
    public class HomepageCacheService : IHomepageCacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IServiceProvider _serviceProvider;
        private const string CacheKey = "HomepageData_CacheKey";
        private const int PageSize = 20;

        public HomepageCacheService(IMemoryCache memoryCache, IServiceProvider serviceProvider)
        {
            _memoryCache = memoryCache;
            _serviceProvider = serviceProvider;
        }

        public HomepageData? GetHomepageData()
        {
            _memoryCache.TryGetValue(CacheKey, out HomepageData? data);
            return data;
        }

        public void SetHomepageData(HomepageData data)
        {
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromHours(25));

            _memoryCache.Set(CacheKey, data, cacheOptions);
        }

        public async Task<HomepageData> LoadHomepageFromDatabaseAsync(CancellationToken cancellationToken = default)
        {
            using var scope = _serviceProvider.CreateScope();
            var homeRepo = scope.ServiceProvider.GetRequiredService<IHomeRepository>();

            var data = new HomepageData
            {
                TrendingMovies = await LoadMovieCategoryAsync(homeRepo, "trending", cancellationToken),
                TrendingTvShows = await LoadTvCategoryAsync(homeRepo, "trending", cancellationToken),
                NowPlaying = await LoadMovieCategoryAsync(homeRepo, "now_playing", cancellationToken),
                Upcoming = await LoadMovieCategoryAsync(homeRepo, "upcoming", cancellationToken),
                TopRatedMovies = await LoadMovieCategoryAsync(homeRepo, "top_rated", cancellationToken),
                TopRatedTvShows = await LoadTvCategoryAsync(homeRepo, "top_rated", cancellationToken),
                PopularMovies = await LoadMovieCategoryAsync(homeRepo, "popular", cancellationToken),
                PopularTvShows = await LoadTvCategoryAsync(homeRepo, "popular", cancellationToken)
            };

            SetHomepageData(data);
            return data;
        }

        private static async Task<List<MovieApiResult>> LoadMovieCategoryAsync(
            IHomeRepository homeRepo,
            string category,
            CancellationToken cancellationToken)
        {
            var (items, _) = await homeRepo.GetCategoryItemsAsync(category, "movie", 1, PageSize);
            return items.Select(MapToMovieApiResult).ToList();
        }

        private static async Task<List<TvShowApiResult>> LoadTvCategoryAsync(
            IHomeRepository homeRepo,
            string category,
            CancellationToken cancellationToken)
        {
            var (items, _) = await homeRepo.GetCategoryItemsAsync(category, "tv", 1, PageSize);
            return items.Select(MapToTvShowApiResult).ToList();
        }

        private static MovieApiResult MapToMovieApiResult(MediaHomeViewModel item)
        {
            string relativePosterPath = ExtractRelativePath(item.PosterPath, "w500");
            string relativeBackdropPath = ExtractRelativePath(item.BackdropPath, "original");
            string releaseDate = item.ReleaseYear > 0 ? $"{item.ReleaseYear}-01-01" : string.Empty;

            return new MovieApiResult
            {
                Id = item.TmdbId,
                Title = item.Title,
                ReleaseDate = releaseDate,
                Description = item.Description ?? string.Empty,
                PosterPath = relativePosterPath,
                BackdropPath = relativeBackdropPath,
                Genre_ids = new List<int> { item.GenreId },
                VoteAverage = item.Rating
            };
        }

        private static TvShowApiResult MapToTvShowApiResult(MediaHomeViewModel item)
        {
            string relativePosterPath = ExtractRelativePath(item.PosterPath, "w500");
            string relativeBackdropPath = ExtractRelativePath(item.BackdropPath, "original");
            string firstAirDate = item.ReleaseYear > 0 ? $"{item.ReleaseYear}-01-01" : string.Empty;

            return new TvShowApiResult
            {
                Id = item.TmdbId,
                Title = item.Title,
                FirstAirDate = firstAirDate,
                Description = item.Description ?? string.Empty,
                PosterPath = relativePosterPath,
                BackdropPath = relativeBackdropPath,
                GenreIds = new List<int> { item.GenreId },
                Rating = item.Rating
            };
        }

        private static string ExtractRelativePath(string? path, string sizeSegment)
        {
            if (string.IsNullOrWhiteSpace(path))
                return string.Empty;

            string tmdbPrefix = $"https://image.tmdb.org/t/p/{sizeSegment}";
            if (path.StartsWith(tmdbPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return path.Substring(tmdbPrefix.Length);
            }

            const string genericTmdbPrefix = "https://image.tmdb.org/t/p/";
            if (path.StartsWith(genericTmdbPrefix, StringComparison.OrdinalIgnoreCase))
            {
                int afterSize = path.IndexOf('/', genericTmdbPrefix.Length);
                if (afterSize > 0)
                {
                    return path.Substring(afterSize);
                }
            }

            if (path.StartsWith("https://placehold.co", StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }

            return path;
        }
    }
}
