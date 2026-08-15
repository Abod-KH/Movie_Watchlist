namespace Movie_Watchlist.Application.ViewModels
{
    public class TvShowWatchlistDashboardViewModel
    {
        public IEnumerable<TvShowWatchlistViewModel> TvShows { get; set; } = new List<TvShowWatchlistViewModel>();
        public int TotalCount { get; set; }
        public int WatchedCount { get; set; }
        public int PageNumber { get; set; } = 1;
        public int TotalPages { get; set; }
        
        public double WatchedPercentage => TotalCount > 0 ? (double)WatchedCount / TotalCount * 100 : 0;
    }
}
