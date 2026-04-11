using Movie_Watchlist.Domain.Entities;

namespace Movie_Watchlist.Application.ViewModels
{
    public class MovieDisplayModel
    {
 
        public IEnumerable<MovieHomeViewModel> Movies { get; set; }

       
        public IEnumerable<Genre> Genres { get; set; }

       
        public string STerm { get; set; } = "";
        public int GenreId { get; set; } = 0;
    }
}
