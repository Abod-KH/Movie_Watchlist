using Movie_Watchlist.Application.DTOs;

namespace Movie_Watchlist.Application.ViewModels
{
    public class HomepageData
    {
        public List<MovieApiResult> TrendingMovies { get; set; } = new();
        public List<TvShowApiResult> TrendingTvShows { get; set; } = new();
        public List<MovieApiResult> NowPlaying { get; set; } = new();
        public List<MovieApiResult> Upcoming { get; set; } = new();
        public List<MovieApiResult> TopRatedMovies { get; set; } = new();
        public List<TvShowApiResult> TopRatedTvShows { get; set; } = new();
        public List<MovieApiResult> PopularMovies { get; set; } = new();
        public List<TvShowApiResult> PopularTvShows { get; set; } = new();
    }
}
