using Movie_Watchlist.Application.DTOs;

namespace Movie_Watchlist.Application.ViewModels
{
    public class ShowMoreViewModel
    {
        public string SectionTitle { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public List<MovieApiResult> Movies { get; set; } = new();
        public List<TvShowApiResult> TvShows { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public bool IsMovieSection { get; set; }
    }
}
