namespace Movie_Watchlist.Application.ViewModels
{
    public class MovieHomeViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int TmdbId { get; set; }
        public string? Description { get; set; }
        public string? PosterPath { get; set; }
        public int ReleaseYear { get; set; }
        public int GenreId { get; set; }
        public bool IsInWatchlist { get; set; }
        public string GenreName { get; set; }
    }
}
