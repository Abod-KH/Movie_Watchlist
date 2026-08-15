using Movie_Watchlist.Domain.Entities;

namespace Movie_Watchlist.Application.ViewModels
{
    public class MovieDisplayModel
    {
        public IEnumerable<MovieHomeViewModel> Movies { get; set; } = [];

        public MovieFilterViewModel Filter { get; set; } = new();
    }
}
