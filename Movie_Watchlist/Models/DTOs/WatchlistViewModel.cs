namespace Movie_Watchlist.Models.DTOs
{
    public class WatchlistViewModel
    {
        public int MovieId { get; set; }
        public string Title { get; set; }
        public string PosterPath { get; set; }
        public int ReleaseYear { get; set; }
        public bool IsWatched { get; set; }
    }
}
