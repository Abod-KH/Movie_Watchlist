namespace Movie_Watchlist.Application.ViewModels
{
    public class MediaHomeViewModel
    {
        public int Id { get; set; }
        public int TmdbId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? PosterPath { get; set; }
        public double Rating { get; set; }
        public int ReleaseYear { get; set; }
        public string MediaType { get; set; } = string.Empty; // 'movie' or 'tv'
        public string? Description { get; set; }
        public int GenreId { get; set; }
    }
}
