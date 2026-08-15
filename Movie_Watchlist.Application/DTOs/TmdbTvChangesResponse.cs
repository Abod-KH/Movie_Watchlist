using System.Text.Json.Serialization;

namespace Movie_Watchlist.Application.DTOs
{
    public class TmdbTvChangesResponse
    {
        [JsonPropertyName("results")]
        public List<TmdbChangeItem> Results { get; set; } = new();
    }
}
