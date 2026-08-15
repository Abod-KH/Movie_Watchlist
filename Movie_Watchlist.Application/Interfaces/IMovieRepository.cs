using Movie_Watchlist.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Movie_Watchlist.Application.Interfaces
{
     public interface IMovieRepository
    {
        Task InsertOrUpdateAsync(IEnumerable<Movie> movies);
    }
}
