using System.Text.Json.Serialization;

namespace Movie_Watchlist.Application.DTOs
{
    public class TmdbChangesResponse
    {
        [JsonPropertyName("results")]
        public List<TmdbChangeItem>? Results { get; set; }

        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("total_pages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("total_results")]
        public int TotalResults { get; set; }
    }
}
