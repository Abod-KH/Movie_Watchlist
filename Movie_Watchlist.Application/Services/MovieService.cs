using Movie_Watchlist.Application.Interfaces;
using Movie_Watchlist.Application.Models;
using Movie_Watchlist.Domain.Entities;


namespace Movie_Watchlist.Application.Services
{


    public class MovieService : IMovieService
    {
        private readonly ITmdbService _tmdbService;
        private readonly IMovieRepository _movieRepository;

        public MovieService(ITmdbService tmdbService, IMovieRepository movieRepository)
        {
            _tmdbService = tmdbService;
            _movieRepository = movieRepository;
        }

        public async Task ImportMoviesAsync()
        {
            var apiMovies = await _tmdbService.GetPopularMoviesAsync();

            if (apiMovies == null || !apiMovies.Any())
                return;

            var movies = new List<Movie>();

            foreach (var apiMovie in apiMovies)
            {
                int genreToUse = (apiMovie.Genre_ids != null && apiMovie.Genre_ids.Any())
                        ? apiMovie.Genre_ids.First()
                        : 28;

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
            }

            await _movieRepository.InsertOrUpdateAsync(movies);
        }
    }
}
