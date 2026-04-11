using Movie_Watchlist.Application.DTOs;

namespace Movie_Watchlist.Application.Models
{
    
    public class TmdbSearchResponse
    {
        public List<MovieApiResult> Results { get; set; }
    }
}
