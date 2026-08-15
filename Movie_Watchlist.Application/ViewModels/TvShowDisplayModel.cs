using Movie_Watchlist.Domain.Entities;

namespace Movie_Watchlist.Application.ViewModels
{
    public class TvShowDisplayModel
    {
        public IEnumerable<TvShowHomeViewModel> TvShows { get; set; } = new List<TvShowHomeViewModel>();
        public IEnumerable<Genre> Genres { get; set; } = new List<Genre>();
        public int GenreId { get; set; } = 0;
        public string STerm { get; set; } = "";
        
        public int PageNumber { get; set; } = 1;
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
    }
}
