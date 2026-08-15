using System.Text.Json.Serialization;

namespace Movie_Watchlist.Application.DTOs
{
    public class TvShowApiResult
    {
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string? Title { get; set; }

        [JsonPropertyName("first_air_date")]
        public string? FirstAirDate { get; set; }

        [JsonPropertyName("overview")]
        public string? Description { get; set; }

        [JsonPropertyName("poster_path")]
        public string? PosterPath { get; set; }

        [JsonPropertyName("vote_average")]
        public double Rating { get; set; }

        [JsonPropertyName("genre_ids")]
        public List<int>? GenreIds { get; set; }

        [JsonPropertyName("backdrop_path")]
        public string? BackdropPath { get; set; }

        public string FullBackdropPath => string.IsNullOrEmpty(BackdropPath)
            ? "https://placehold.co/1920x1080?text=No+Backdrop"
            : $"https://image.tmdb.org/t/p/original{BackdropPath}";

        public string FullPosterPath => string.IsNullOrEmpty(PosterPath)
            ? "https://placehold.co/500x750?text=No+Poster"
            : $"https://image.tmdb.org/t/p/w500{PosterPath}";
    }
}
