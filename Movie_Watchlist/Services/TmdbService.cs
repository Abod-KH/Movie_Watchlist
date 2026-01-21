using Microsoft.AspNetCore.Mvc;
using Movie_Watchlist.Models;

namespace Movie_Watchlist.Services
{
    public class TmdbService : ITmdbService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "e7b394e9c456eaab403ea5e754a069a1";
       
        public TmdbService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
       
        public async Task<IEnumerable<MovieApiResult>> GetPopularMoviesAsync()
        {
            Random rng = new Random();
            int randomPage = rng.Next(1, 501);

            // TMDB returns 20 movies per page by default
            var url = $"https://api.themoviedb.org/3/movie/popular?api_key={_apiKey}&language=en-US&page={randomPage}";
            var response = await _httpClient.GetFromJsonAsync<TmdbSearchResponse>(url);
            return response?.Results ?? new List<MovieApiResult>();
        }

        public async Task<IEnumerable<MovieApiResult>> SearchMoviesAsync(string query)
        {
            var url = $"https://api.themoviedb.org/3/search/movie?api_key={_apiKey}&query={Uri.EscapeDataString(query)}";
            var response = await _httpClient.GetFromJsonAsync<TmdbSearchResponse>(url);
            return response?.Results ?? new List<MovieApiResult>();
        }
    }
}
