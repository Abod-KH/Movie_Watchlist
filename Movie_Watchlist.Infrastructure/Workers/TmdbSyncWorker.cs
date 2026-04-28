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

            while (!stoppingToken.IsCancellationRequested)
            {
                var delay = CalculateDelayUntilNextRun(16, 55); 
                _logger.LogInformation($"TmdbSyncWorker is waiting for {delay.TotalHours:F2} hours until the next run.");

                try
                {
                    await Task.Delay(delay, stoppingToken);

                    _logger.LogInformation("TmdbSyncWorker is starting the daily sync.");
                    await PerformDailySyncAsync(stoppingToken);
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
            var tmdbService = scope.ServiceProvider.GetRequiredService<ITmdbService>();

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
                        if (!string.IsNullOrWhiteSpace(apiMovie.ReleaseDate))
                        {
                            var parts = apiMovie.ReleaseDate.Split('-');
                            if (parts.Length > 0 && int.TryParse(parts[0], out var y))
                                releaseYear = y;
                        }

                        movies.Add(new Movie
                        {
                            Title = apiMovie.Title,
                            TmdbId = apiMovie.Id,
                            Description = apiMovie.Description,
                            PosterPath = apiMovie.FullPosterPath,
                            ReleaseYear = releaseYear,
                            GenreId = genreToUse
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
