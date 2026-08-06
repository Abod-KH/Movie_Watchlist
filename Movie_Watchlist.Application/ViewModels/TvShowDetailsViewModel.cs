using Movie_Watchlist.Application.DTOs;
using Movie_Watchlist.Domain.Entities;

namespace Movie_Watchlist.Application.ViewModels
{
    public class TvShowDetailsViewModel
    {
        public TvShow TvShow { get; set; } = new();
        public string? TrailerKey { get; set; }
        public List<TvShowApiResult> SimilarTvShows { get; set; } = new();
    }
}
