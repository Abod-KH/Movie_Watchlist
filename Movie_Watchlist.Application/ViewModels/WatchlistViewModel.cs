namespace Movie_Watchlist.Application.ViewModels
{
    public class WatchlistViewModel
    {
        public int MovieId { get; set; }
        public string? Title { get; set; }
        public string? PosterPath { get; set; }
        public int ReleaseYear { get; set; }
        public double VoteAverage { get; set; }
        public bool IsWatched { get; set; }
        public int TotalCount { get; set; }
        public int WatchedCount { get; set; }
    }
}
