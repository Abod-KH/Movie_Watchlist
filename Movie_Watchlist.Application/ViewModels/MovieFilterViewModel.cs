using Movie_Watchlist.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Movie_Watchlist.Application.ViewModels
{
    public class MovieFilterViewModel
    {
        public IEnumerable<Genre> Genres { get; set; } = [];

        public string STerm { get; set; } = string.Empty;

        public int GenreId { get; set; } = 0;
    }
}
