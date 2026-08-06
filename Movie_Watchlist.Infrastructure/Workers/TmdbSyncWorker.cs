using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


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
            await RefreshHomepageCacheAsync(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                var delay = CalculateDelayUntilNextRun(14,55); 
                _logger.LogInformation($"TmdbSyncWorker is waiting for {delay.TotalHours:F2} hours until the next run.");

                try
                {
                    await Task.Delay(delay, stoppingToken);

                    _logger.LogInformation("TmdbSyncWorker is starting the daily sync.");
                    await PerformDailySyncAsync(stoppingToken);
                    
                    _logger.LogInformation("TmdbSyncWorker is refreshing homepage cache.");
                    await RefreshHomepageCacheAsync(stoppingToken);
                    
                    _logger.LogInformation("TmdbSyncWorker completed the daily sync.");
                }
                catch (TaskCanceledException)
                {
                    _logger.LogInformation("TmdbSyncWorker delay was cancelled.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred during the daily sync.");
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

            var data = new HomepageData();

            try
            {
                var trendingMovies = await tmdbService.GetTrendingMoviesAsync(1);
                if (trendingMovies != null) data.TrendingMovies = trendingMovies.Results.Take(20).ToList();
                await Task.Delay(250, stoppingToken);

                var trendingTv = await tmdbService.GetTrendingTvShowsAsync(1);
                if (trendingTv != null) data.TrendingTvShows = trendingTv.Results.Take(20).ToList();
                await Task.Delay(250, stoppingToken);

                var nowPlaying = await tmdbService.GetNowPlayingMoviesAsync(1);
                if (nowPlaying != null) data.NowPlaying = nowPlaying.Results.Take(20).ToList();
                await Task.Delay(250, stoppingToken);

                var upcoming = await tmdbService.GetUpcomingMoviesAsync(1);
                if (upcoming != null) data.Upcoming = upcoming.Results.Take(20).ToList();
                await Task.Delay(250, stoppingToken);

                var topRatedMovies = await tmdbService.GetTopRatedMoviesAsync(1);
                if (topRatedMovies != null) data.TopRatedMovies = topRatedMovies.Results.Take(20).ToList();
                await Task.Delay(250, stoppingToken);

                var topRatedTv = await tmdbService.GetTopRatedTvShowsAsync(1);
                if (topRatedTv != null) data.TopRatedTvShows = topRatedTv.Results.Take(20).ToList();
                await Task.Delay(250, stoppingToken);

                var popularMovies = await tmdbService.GetPopularMoviesAsync(1);
                if (popularMovies != null) data.PopularMovies = popularMovies.Results.Take(20).ToList();
                await Task.Delay(250, stoppingToken);

                var popularTv = await tmdbService.GetPopularTvShowsAsync(1);
                if (popularTv != null) data.PopularTvShows = popularTv.Results.Take(20).ToList();

                cacheService.SetHomepageData(data);
                _logger.LogInformation("Successfully refreshed homepage cache.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to refresh homepage cache.");
            }
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
