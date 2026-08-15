using System;
using System.Collections.Generic;
using System.Text;

namespace Movie_Watchlist.Application.DTOs
{
    public class TmdbSimilarMoviesResponse
    {
        public List<MovieApiResult>? Results { get; set; }
    }
}
