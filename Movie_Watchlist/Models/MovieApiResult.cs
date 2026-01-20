using System.Text.Json.Serialization;

namespace Movie_Watchlist.Models
{
    public class MovieApiResult
    {
        public int Id { get; set; }
        public string Title { get; set; }

        [JsonPropertyName("release_date")] 
        public string ReleaseDate { get; set; }

        [JsonPropertyName("overview")]
        public string? Description { get; set; }

        [JsonPropertyName("poster_path")]
        public string PosterPath { get; set; }

        [JsonPropertyName("genre_ids")]
        public List<int> Genre_ids { get; set; }



        // This is the full URL for your <img> tags
        public string FullPosterPath => string.IsNullOrEmpty(PosterPath)
            ? "https://placehold.co/500x750?text=No+Poster"
            : $"https://image.tmdb.org/t/p/w500{PosterPath}";
    }
}
