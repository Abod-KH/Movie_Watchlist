using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Movie_Watchlist.Application.DTOs;
using Movie_Watchlist.Application.Interfaces;
using Movie_Watchlist.Domain.Entities;
using Movie_Watchlist.Application.ViewModels;
namespace Movie_Watchlist.Infrastructure.Workers
{
    public class TmdbSyncWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TmdbSyncWorker> _logger;

        public TmdbSyncWorker(IServiceProvider serviceProvider, ILogger<TmdbSyncWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("TmdbSyncWorker is starting.");

            // Populate cache immediately on startup
            _logger.LogInformation("Populating homepage cache on startup...");
            try
            {
                await RefreshHomepageCacheAsync(stoppingToken);
                _logger.LogInformation("Startup cache refresh completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed during startup cache refresh. Will retry on schedule.");
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                // For testing: run every 5 minutes. Change back to CalculateDelayUntilNextRun(23, 16) for production
                var delay = TimeSpan.FromMinutes(30);
                _logger.LogInformation($"TmdbSyncWorker is waiting for {delay.TotalMinutes:F1} minutes until the next run.");

                try
                {
                    await Task.Delay(delay, stoppingToken);

                   // _logger.LogInformation("TmdbSyncWorker is starting the daily sync.");
                   // await PerformDailySyncAsync(stoppingToken);

                    _logger.LogInformation("TmdbSyncWorker is refreshing homepage cache.");
                    await RefreshHomepageCacheAsync(stoppingToken);

                    _logger.LogInformation("TmdbSyncWorker completed the cache refresh.");
                }
                catch (TaskCanceledException)
                {
                    _logger.LogInformation("TmdbSyncWorker delay was cancelled.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred during cache refresh.");
                }
            }
        }

        private async Task PerformDailySyncAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var movieRepo = scope.ServiceProvider.GetRequiredService<IMovieRepository>();
            var tvShowRepo = scope.ServiceProvider.GetRequiredService<ITvShowRepository>();
            var tmdbService = scope.ServiceProvider.GetRequiredService<ITmdbService>();

            // await SyncMoviesAsync(movieRepo, tmdbService, stoppingToken);
            await SyncTvShowsAsync(tvShowRepo, tmdbService, stoppingToken);
        }

        private async Task SyncMoviesAsync(IMovieRepository movieRepo, ITmdbService tmdbService, CancellationToken stoppingToken)
        {
            var changedIds = await tmdbService.GetChangedMovieIdsAsync();
            if (changedIds == null || !changedIds.Any())
            {
                _logger.LogInformation("No changed movies found in the last 24 hours.");
                return;
            }

            _logger.LogInformation($"Found {changedIds.Count()} changed movies. Processing...");

            var movies = new List<Movie>();

            foreach (var id in changedIds)
            {
                if (stoppingToken.IsCancellationRequested) break;

                try
                {
                    var apiMovie = await tmdbService.GetMovieDetailsAsync(id);
                    if (apiMovie != null)
                    {
                        int genreToUse = (apiMovie.Genre_ids != null && apiMovie.Genre_ids.Any()) ? apiMovie.Genre_ids.First() : 28;
                        int releaseYear = 0;
                        if (DateTime.TryParse(apiMovie.ReleaseDate, out var date))
                        {
                            releaseYear = date.Year;
                        }

                        movies.Add(new Movie
                        {
                            Title = apiMovie.Title,
                            TmdbId = apiMovie.Id,
                            Description = apiMovie.Description,
                            PosterPath = apiMovie.FullPosterPath,
                            ReleaseYear = releaseYear,
                            GenreId = genreToUse,
                            VoteAverage = apiMovie.VoteAverage
                        });

                        if (movies.Count >= 50)
                        {
                            await movieRepo.InsertOrUpdateAsync(movies);
                            movies.Clear();
                        }
                    }

                    await Task.Delay(250, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to process changed movie ID {id}.");
                }
            }

            if (movies.Any())
            {
                await movieRepo.InsertOrUpdateAsync(movies);
            }
        }

        private async Task SyncTvShowsAsync(ITvShowRepository tvShowRepo, ITmdbService tmdbService, CancellationToken stoppingToken)
        {
            var changedIds = await tmdbService.GetChangedTvShowIdsAsync();
            if (changedIds == null || !changedIds.Any())
            {
                _logger.LogInformation("No changed TV shows found in the last 24 hours.");
                return;
            }

            _logger.LogInformation($"Found {changedIds.Count()} changed TV shows. Processing...");

            var tvShows = new List<TvShow>();

            foreach (var id in changedIds)
            {
                if (stoppingToken.IsCancellationRequested) break;

                try
                {
                    var apiTvShow = await tmdbService.GetTvShowDetailsAsync(id);
                    if (apiTvShow != null)
                    {
                        int genreToUse = (apiTvShow.GenreIds != null && apiTvShow.GenreIds.Any()) ? apiTvShow.GenreIds.First() : 18; // 18 is Drama
                        int releaseYear = 0;
                        if (DateTime.TryParse(apiTvShow.FirstAirDate, out var date))
                        {
                            releaseYear = date.Year;
                        }

                        tvShows.Add(new TvShow
                        {
                            Title = apiTvShow.Title ?? "Unknown",
                            TmdbId = apiTvShow.Id,
                            Description = apiTvShow.Description ?? "",
                            PosterPath = apiTvShow.FullPosterPath,
                            ReleaseYear = releaseYear,
                            GenreId = genreToUse,
                            Rating = apiTvShow.Rating
                        });

                        if (tvShows.Count >= 50)
                        {
                            await tvShowRepo.InsertOrUpdateAsync(tvShows);
                            tvShows.Clear();
                        }
                    }

                    await Task.Delay(250, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to process changed TV show ID {id}.");
                }
            }

            if (tvShows.Any())
            {
                await tvShowRepo.InsertOrUpdateAsync(tvShows);
            }
        }

        private async Task RefreshHomepageCacheAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var tmdbService = scope.ServiceProvider.GetRequiredService<ITmdbService>();
            var cacheService = scope.ServiceProvider.GetRequiredService<IHomepageCacheService>();
            var movieRepo = scope.ServiceProvider.GetRequiredService<IMovieRepository>();
            var tvShowRepo = scope.ServiceProvider.GetRequiredService<ITvShowRepository>();
            var homeRepo = scope.ServiceProvider.GetRequiredService<IHomeRepository>();

            var data = new HomepageData();

            try
            {
                data.TrendingMovies = await FetchAndSyncCategoryMoviesAsync("trending", page => tmdbService.GetTrendingMoviesAsync(page), movieRepo, homeRepo, stoppingToken);
                data.TrendingTvShows = await FetchAndSyncCategoryTvShowsAsync("trending", page => tmdbService.GetTrendingTvShowsAsync(page), tvShowRepo, homeRepo, stoppingToken);
                
                data.NowPlaying = await FetchAndSyncCategoryMoviesAsync("now_playing", page => tmdbService.GetNowPlayingMoviesAsync(page), movieRepo, homeRepo, stoppingToken);
                data.Upcoming = await FetchAndSyncCategoryMoviesAsync("upcoming", page => tmdbService.GetUpcomingMoviesAsync(page), movieRepo, homeRepo, stoppingToken);
                
                data.TopRatedMovies = await FetchAndSyncCategoryMoviesAsync("top_rated", page => tmdbService.GetTopRatedMoviesAsync(page), movieRepo, homeRepo, stoppingToken);
                data.TopRatedTvShows = await FetchAndSyncCategoryTvShowsAsync("top_rated", page => tmdbService.GetTopRatedTvShowsAsync(page), tvShowRepo, homeRepo, stoppingToken);
                
                data.PopularMovies = await FetchAndSyncCategoryMoviesAsync("popular", page => tmdbService.GetPopularMoviesAsync(page), movieRepo, homeRepo, stoppingToken);
                data.PopularTvShows = await FetchAndSyncCategoryTvShowsAsync("popular", page => tmdbService.GetPopularTvShowsAsync(page), tvShowRepo, homeRepo, stoppingToken);

                cacheService.SetHomepageData(data);
                _logger.LogInformation("Successfully refreshed homepage cache.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to refresh homepage cache.");
            }
        }

        private async Task<List<MovieApiResult>> FetchAndSyncCategoryMoviesAsync(
            string category, 
            Func<int, Task<TmdbPagedResponse<MovieApiResult>?>> fetchFunc, 
            IMovieRepository movieRepo, 
            IHomeRepository homeRepo, 
            CancellationToken stoppingToken)
        {
            var allApiResults = new List<MovieApiResult>();
            var mappings = new List<object>();
            var moviesToSave = new List<Movie>();

            for (int page = 1; page <= 5; page++)
            {
                var response = await fetchFunc(page);
                if (response?.Results != null)
                {
                    allApiResults.AddRange(response.Results);
                }
                await Task.Delay(250, stoppingToken);
            }

            // Deduplicate by TmdbId to avoid duplicate entries in the database
            var uniqueMovies = allApiResults
                .GroupBy(m => m.Id)
                .Select(g => g.First())
                .ToList();

            int rank = 1;
            foreach (var apiMovie in uniqueMovies)
            {
                // Get the first genre from TMDB, default to Action (28) if none found
                int tmdbGenreId = (apiMovie.Genre_ids != null && apiMovie.Genre_ids.Any()) ? apiMovie.Genre_ids.First() : 28;
                // Validate and map to a valid local database genre
                int genreToUse = MapTmdbGenreToLocalGenre(tmdbGenreId);

                int releaseYear = 0;
                if (DateTime.TryParse(apiMovie.ReleaseDate, out var date)) releaseYear = date.Year;

                moviesToSave.Add(new Movie
                {
                    Title = apiMovie.Title ?? "Unknown",
                    TmdbId = apiMovie.Id,
                    Description = apiMovie.Description ?? "",
                    PosterPath = apiMovie.FullPosterPath,
                    ReleaseYear = releaseYear,
                    GenreId = genreToUse,
                    VoteAverage = apiMovie.VoteAverage
                });

                mappings.Add(new { TmdbId = apiMovie.Id, Rank = rank++ });
            }

            if (moviesToSave.Any())
            {
                await movieRepo.InsertOrUpdateAsync(moviesToSave);
                var mappingsJson = System.Text.Json.JsonSerializer.Serialize(mappings);
                await homeRepo.UpdateCategoryMappingsAsync(category, "movie", mappingsJson);
            }

            return uniqueMovies.Take(20).ToList();
        }

        private async Task<List<TvShowApiResult>> FetchAndSyncCategoryTvShowsAsync(
            string category, 
            Func<int, Task<TmdbPagedResponse<TvShowApiResult>?>> fetchFunc, 
            ITvShowRepository tvShowRepo, 
            IHomeRepository homeRepo, 
            CancellationToken stoppingToken)
        {
            var allApiResults = new List<TvShowApiResult>();
            var mappings = new List<object>();
            var tvShowsToSave = new List<TvShow>();

            for (int page = 1; page <= 5; page++)
            {
                var response = await fetchFunc(page);
                if (response?.Results != null)
                {
                    allApiResults.AddRange(response.Results);
                }
                await Task.Delay(250, stoppingToken);
            }

            // Deduplicate by TmdbId to avoid duplicate entries in the database
            var uniqueTvShows = allApiResults
                .GroupBy(t => t.Id)
                .Select(g => g.First())
                .ToList();

            int rank = 1;
            foreach (var apiTvShow in uniqueTvShows)
            {
                // Get the first genre from TMDB, default to Drama (18) if none found
                int tmdbGenreId = (apiTvShow.GenreIds != null && apiTvShow.GenreIds.Any()) ? apiTvShow.GenreIds.First() : 18;
                // Validate and map to a valid local database genre
                int genreToUse = MapTmdbGenreToLocalGenre(tmdbGenreId);

                int releaseYear = 0;
                if (DateTime.TryParse(apiTvShow.FirstAirDate, out var date)) releaseYear = date.Year;

                tvShowsToSave.Add(new TvShow
                {
                    Title = apiTvShow.Title ?? "Unknown",
                    TmdbId = apiTvShow.Id,
                    Description = apiTvShow.Description ?? "",
                    PosterPath = apiTvShow.FullPosterPath,
                    ReleaseYear = releaseYear,
                    GenreId = genreToUse,
                    Rating = apiTvShow.Rating
                });

                mappings.Add(new { TmdbId = apiTvShow.Id, Rank = rank++ });
            }

            if (tvShowsToSave.Any())
            {
                await tvShowRepo.InsertOrUpdateAsync(tvShowsToSave);
                var mappingsJson = System.Text.Json.JsonSerializer.Serialize(mappings);
                await homeRepo.UpdateCategoryMappingsAsync(category, "tv", mappingsJson);
            }

            return uniqueTvShows.Take(20).ToList();
        }

        private int MapTmdbGenreToLocalGenre(int tmdbGenreId)
        {
            // Map TMDB genre IDs to your local database genre IDs
            // Based on your database genres
            var genreMapping = new Dictionary<int, int>
            {
                // TMDB ID -> Local DB ID
                { 12, 14 },   // Adventure -> Adventure
                { 14, 14 },   // Fantasy -> Fantasy
                { 16, 16 },   // Animation -> Animation
                { 18, 18 },   // Drama -> Drama
                { 27, 27 },   // Horror -> Horror
                { 28, 28 },   // Action -> Action
                { 35, 35 },   // Comedy -> Comedy
                { 36, 36 },   // History -> History
                { 37, 37 },   // Western -> Western
                { 53, 53 },   // Thriller -> Thriller
                { 80, 80 },   // Crime -> Crime
                { 99, 99 },   // Documentary -> Documentary
                { 878, 878 }, // Science Fiction -> Science Fiction
                { 9648, 9648 }, // Mystery -> Mystery
                { 10402, 10402 }, // Music -> Music
                { 10749, 10749 }, // Romance -> Romance
                { 10751, 10751 }, // Family -> Family
                { 10752, 10752 }, // War -> War
            };

            // If the TMDB genre ID is in our mapping, use it
            if (genreMapping.TryGetValue(tmdbGenreId, out var localGenreId))
            {
                return localGenreId;
            }

            // Fallback: default to Action (28) for movies or Drama (18) for TV shows
            // Since this method is used for both, we'll use a safe genre that exists
            return 28; // Action is a common genre for both movies and TV shows
        }

        private TimeSpan CalculateDelayUntilNextRun(int targetHour, int targetMinute)
        {
            var now = DateTime.Now;
            var nextRun = new DateTime(now.Year, now.Month, now.Day, targetHour, targetMinute, 0);

            if (now >= nextRun)
            {
                nextRun = nextRun.AddDays(1);
            }

            return nextRun - now;
        }
    }
}
