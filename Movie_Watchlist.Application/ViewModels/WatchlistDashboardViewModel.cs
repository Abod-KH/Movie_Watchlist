using Movie_Watchlist.Application.ViewModels;

namespace Movie_Watchlist.Application.Models
{
    public class WatchlistDashboardViewModel
    {
    
        public int TotalMovies { get; set; }
        public int MoviesWatched { get; set; }
        public int Percentage { get; set; }

        
        public IEnumerable<WatchlistViewModel> Movies { get; set; }
    }
}