using System.Text.Json.Serialization;

namespace Movie_Watchlist.Application.DTOs
{
    public class TmdbChangeItem
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        
    }
}
