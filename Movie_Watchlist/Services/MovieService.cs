using Movie_Watchlist.Data;
using Movie_Watchlist.Models;

namespace Movie_Watchlist.Services
{
 

    public class MovieService : IMovieService
    {
        private readonly ITmdbService _tmdbService;
        private readonly ApplicationDbContext _db;

        public MovieService(ITmdbService tmdbService, ApplicationDbContext db)
        {
            _tmdbService = tmdbService;
            _db = db;
        }

        public async Task ImportMoviesAsync()
        {
            var apiMovies = await _tmdbService.GetPopularMoviesAsync();
            foreach (var apiMovie in apiMovies)
            {
                if (!_db.Movies.Any(m => m.TmdbId == apiMovie.Id))
                {
                    int genreToUse = (apiMovie.Genre_ids != null && apiMovie.Genre_ids.Any())
                         ? apiMovie.Genre_ids.First()
                         : 28;
                    var movie = new Movie
                    {
                        TmdbId = apiMovie.Id,
                        Title = apiMovie.Title,
                        ReleaseYear = int.TryParse(apiMovie.ReleaseDate?.Split('-')[0], out int y) ? y : 0,
                        GenreId = genreToUse,
                        Description = apiMovie.Description,
                        PosterPath = apiMovie.FullPosterPath
                    };
                    _db.Movies.Add(movie);
                }
            }
            await _db.SaveChangesAsync();
        }
    }
}
