namespace Movie_Watchlist.Domain.Entities
{
    public class TvShow
    {
        public int Id { get; set; }
        public int TmdbId { get; set; }
        public string Title { get; set; } = "Unknown";
        public string Description { get; set; } = string.Empty;
        public string? PosterPath { get; set; }
        public string? BackdropPath { get; set; }
        public double Rating { get; set; }
        public int ReleaseYear { get; set; }
        public int GenreId { get; set; }
        public Genre? Genre { get; set; }
        public string? GenreName { get; set; }
    }
}
