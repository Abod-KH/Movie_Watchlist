
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Movie_Watchlist.Application.Interfaces;
using Movie_Watchlist.Application.Models;
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
       
        public async Task<IEnumerable<MovieApiResult>> GetPopularMoviesAsync()
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                _logger.LogWarning("TMDB API key is missing. Cannot search movies.");
                return new List<MovieApiResult>();
            }

            Random rng = new Random();
            int randomPage = rng.Next(1, 501);
           

            // TMDB returns 20 movies per page by default
            var url = $"https://api.themoviedb.org/3/movie/popular?api_key={_apiKey}&language=en-US&page={randomPage}";
            try
            {
                var response = await _httpClient.GetFromJsonAsync<TmdbSearchResponse>(url);
                return response?.Results ?? new List<MovieApiResult>();
            }
            catch (HttpRequestException e)
            {
                
                Console.WriteLine($"TMDB request failed: {e.Message}");
                return new List<MovieApiResult>();
            }
        }

        public async Task<IEnumerable<MovieApiResult>> SearchMoviesAsync(string query)
        {
            var url = $"https://api.themoviedb.org/3/search/movie?api_key={_apiKey}&query={Uri.EscapeDataString(query)}";
            var response = await _httpClient.GetFromJsonAsync<TmdbSearchResponse>(url);
            return response?.Results ?? new List<MovieApiResult>();
        }
    }
}
