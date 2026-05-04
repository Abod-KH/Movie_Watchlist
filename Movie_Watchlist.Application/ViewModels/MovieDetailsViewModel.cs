using Movie_Watchlist.Application.DTOs;
using Movie_Watchlist.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Movie_Watchlist.Application.ViewModels
{
    public class MovieDetailsViewModel
    {
        public Movie? Movie { get; set; }
        public string? TrailerKey { get; set; }
        public IEnumerable<MovieApiResult> SimilarMovies { get; set; } = new List<MovieApiResult>();
    }
}
