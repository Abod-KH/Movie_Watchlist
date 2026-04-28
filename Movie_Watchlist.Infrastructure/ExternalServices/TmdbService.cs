
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

        public TmdbService(HttpClient httpClient, IConfiguration config, ILogger<TmdbService> logger)
        {
            _httpClient = httpClient;
            _apiKey = config["Tmdb:ApiKey"];
            _logger = logger;
        }



        public async Task<IEnumerable<int>> GetChangedMovieIdsAsync()
        {
            if (string.IsNullOrEmpty(_apiKey)) return new List<int>();

            var allIds = new List<int>();
            int currentPage = 1;
            int totalPages = 1;

            try
            {
                do
                {
                    var url = $"https://api.themoviedb.org/3/movie/changes?api_key={_apiKey}&page={currentPage}";
                    var response = await _httpClient.GetFromJsonAsync<TmdbChangesResponse>(url);

                    if (response?.Results != null)
                    {
                        allIds.AddRange(response.Results.Select(r => r.Id));
                        totalPages = response.TotalPages; // Update the total pages from the API
                    }

                    currentPage++;

                    
                    if (currentPage >= 50) break;

                } while (currentPage <= totalPages);

                return allIds;
            }
            catch (HttpRequestException e)
            {
                _logger.LogWarning($"TMDB GetChangedMovieIdsAsync failed at page {currentPage}: {e.Message}");
                return allIds; // Return what we managed to collect before the error
            }
        }

        public async Task<MovieApiResult?> GetMovieDetailsAsync(int tmdbId)
        {
            if (string.IsNullOrEmpty(_apiKey))
                return null;

            var url = $"https://api.themoviedb.org/3/movie/{tmdbId}?api_key={_apiKey}&language=en-US";
            try
            {
                return await _httpClient.GetFromJsonAsync<MovieApiResult>(url);
            }
            catch (HttpRequestException e)
            {
                _logger.LogWarning($"TMDB GetMovieDetailsAsync failed for ID {tmdbId}: {e.Message}");
                return null;
            }
        }
    }
}
