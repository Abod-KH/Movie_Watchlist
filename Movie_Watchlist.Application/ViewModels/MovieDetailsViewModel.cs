using Movie_Watchlist.Application.DTOs;
using Movie_Watchlist.Domain.Entities;

namespace Movie_Watchlist.Application.ViewModels
{
    public class MovieDetailsViewModel
    {
        public Movie? Movie { get; set; }
        public string? TrailerKey { get; set; }
        public IEnumerable<MovieApiResult> SimilarMovies { get; set; } = new List<MovieApiResult>();
        public bool IsInWatchlist { get; set; }
    }
}
