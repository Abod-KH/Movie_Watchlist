
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Movie_Watchlist.Application.DTOs;
 
using System.Net.Http.Json;

namespace Movie_Watchlist.Application.Services
{
    public class TmdbService : ITmdbService
    {
        private readonly HttpClient _httpClient;
        private readonly string? _apiKey;
        private readonly ILogger<TmdbService> _logger;
        private const string TmdbUrl = "https://api.themoviedb.org/3";
        public TmdbService(HttpClient httpClient, IConfiguration config, ILogger<TmdbService> logger)
        {
            _httpClient = httpClient;
            _apiKey = config["Tmdb:ApiKey"];
            _logger = logger;
            if (string.IsNullOrEmpty(_apiKey))
            {
                _logger.LogError("TMDB API key is missing. TMDB features will be disabled.");
            }
        }

        private async Task<T?> SafeGetAsync<T>(string url, string context)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<T>(url);
            }
            catch (Exception e)
            {
                _logger.LogError($"{context} failed: {e}");
                return default;
            }
        }


        public async Task<IEnumerable<int>> GetChangedMovieIdsAsync()
        {

            var allIds = new List<int>();
            int currentPage = 1;
            int totalPages = 1;

            do
            {
                var url = $"{TmdbUrl}/movie/changes?api_key={_apiKey}&page={currentPage}";
                var response = await SafeGetAsync<TmdbChangesResponse>(
                    url,
                    $"TMDB GetChangedMovieIdsAsync (page {currentPage})"
                );

                if (response?.Results != null)
                {
                    allIds.AddRange(response.Results.Select(r => r.Id));
                    totalPages = response.TotalPages;
                }
                currentPage++;

                if (currentPage >= 50)
                    break;

            } while (currentPage <= totalPages);

            return allIds;

        } 

        public async Task<MovieApiResult?> GetMovieDetailsAsync(int tmdbId)
        {
            var url = $"{TmdbUrl}/movie/{tmdbId}?api_key={_apiKey}&language=en-US";

            return await SafeGetAsync<MovieApiResult>(
                url,
                $"TMDB GetMovieDetailsAsync (ID {tmdbId})"
            );
        }

        public async Task<string?> GetMovieTrailerKeyAsync(int tmdbId)
        {

            var url = $"{TmdbUrl}/movie/{tmdbId}/videos?api_key={_apiKey}";

            var response = await SafeGetAsync<TmdbVideoResponse>(
                url,
                $"TMDB GetMovieTrailerKeyAsync (ID {tmdbId})"
            );
            return response?.Results?
               .FirstOrDefault(v => v.Site == "YouTube" && v.Type == "Trailer")
               ?.Key;
           
        }

        public async Task<IEnumerable<MovieApiResult>> GetSimilarMoviesAsync(int tmdbId)
        {
            var url = $"{TmdbUrl}/movie/{tmdbId}/similar?api_key={_apiKey}&language=en-US";

            var response = await SafeGetAsync<TmdbSimilarMoviesResponse>(
                url,
                $"TMDB GetSimilarMoviesAsync (ID {tmdbId})"
            );

            return response?.Results?.Take(10) ?? Enumerable.Empty<MovieApiResult>();
        }
    }
}
