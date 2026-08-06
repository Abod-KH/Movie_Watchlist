using Movie_Watchlist.Domain.Entities;

namespace Movie_Watchlist.Application.Interfaces
{
    public interface ITvShowRepository
    {
        Task InsertOrUpdateAsync(IEnumerable<TvShow> tvShows);
    }
}
