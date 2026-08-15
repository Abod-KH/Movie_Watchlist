using System.Text.Json.Serialization;

namespace Movie_Watchlist.Application.DTOs
{
    public class TmdbSimilarTvShowsResponse
    {
        [JsonPropertyName("results")]
        public List<TvShowApiResult> Results { get; set; } = new();
    }
}
