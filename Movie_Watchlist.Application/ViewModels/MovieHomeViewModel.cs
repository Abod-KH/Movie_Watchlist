using Movie_Watchlist.Domain.Entities;

namespace Movie_Watchlist.Application.ViewModels
{
    public class MovieHomeViewModel
    {
        public Movie Movie { get; set; }
        public bool IsInWatchlist { get; set; }
        public string GenreName { get; set; }
    }
}
