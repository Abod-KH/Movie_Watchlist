using Movie_Watchlist.Application.ViewModels;

namespace Movie_Watchlist.Application.ViewModels
{
    public class SearchViewModel
    {
        public List<MediaHomeViewModel> Items { get; set; } = new();
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
