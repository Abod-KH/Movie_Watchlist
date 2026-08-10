using Movie_Watchlist.Application.DTOs;

namespace Movie_Watchlist.Application.ViewModels
{
    public class ShowMoreViewModel
    {
        public string SectionTitle { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public List<MediaHomeViewModel> Items { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public bool IsMovieSection { get; set; }
    }
}
