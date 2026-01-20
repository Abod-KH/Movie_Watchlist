using Movie_Watchlist.Models.DTOs;

namespace Movie_Watchlist.Models
{
    public class WatchlistDashboardViewModel
    {
    
        public int TotalMovies { get; set; }
        public int MoviesWatched { get; set; }
        public int Percentage { get; set; }

        
        public IEnumerable<WatchlistViewModel> Movies { get; set; }
    }
}