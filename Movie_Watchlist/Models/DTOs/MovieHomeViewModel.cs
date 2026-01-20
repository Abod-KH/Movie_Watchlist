namespace Movie_Watchlist.Models.DTOs
{
    public class MovieHomeViewModel
    {
        public Movie Movie { get; set; }
        public bool IsInWatchlist { get; set; }
        public string GenreName { get; set; }
    }
}
