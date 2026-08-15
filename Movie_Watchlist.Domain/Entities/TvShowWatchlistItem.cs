namespace Movie_Watchlist.Domain.Entities
{
    public class TvShowWatchlistItem
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int TvShowId { get; set; }
        public TvShow? TvShow { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.Now;
        public bool IsWatched { get; set; } = false;
    }
}
