using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Movie_Watchlist.Application.DTOs
{
    public class TmdbVideoResult
    {
        [JsonPropertyName("key")]
        public string? Key { get; set; } // This is the YouTube ID (e.g., d967EbLSeMC)

        [JsonPropertyName("site")]
        public string? Site { get; set; } // We want "YouTube"

        [JsonPropertyName("type")]
        public string? Type { get; set; } // We want "Trailer"
    }

    public class TmdbVideoResponse
    {
        [JsonPropertyName("results")]
        public List<TmdbVideoResult> Results { get; set; } = new();
    }
}
