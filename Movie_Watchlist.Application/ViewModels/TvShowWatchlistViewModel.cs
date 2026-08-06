namespace Movie_Watchlist.Application.ViewModels
{
    public class TvShowWatchlistViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int TvShowId { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsWatched { get; set; }
        
        public int TmdbId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? PosterPath { get; set; }
        public double Rating { get; set; }
        public int ReleaseYear { get; set; }
        public int GenreId { get; set; }
        public string? GenreName { get; set; }
    }
}
