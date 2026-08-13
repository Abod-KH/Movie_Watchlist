namespace Movie_Watchlist.Application.ViewModels
{
    public static class MediaCardExtensions
    {
        public static MediaHomeViewModel ToMediaCard(this MovieHomeViewModel movie) => new()
        {
            Id = movie.Id,
            TmdbId = movie.TmdbId,
            Title = movie.Title ?? string.Empty,
            PosterPath = movie.PosterPath,
            Rating = movie.VoteAverage,
            ReleaseYear = movie.ReleaseYear,
            GenreId = movie.GenreId,
            Description = movie.Description,
            MediaType = "movie",
            IsInWatchlist = movie.IsInWatchlist,
            Action = PosterCardAction.ToggleWatchlist
        };

        public static MediaHomeViewModel ToMediaCard(this TvShowHomeViewModel show) => new()
        {
            Id = show.Id,
            TmdbId = show.TmdbId,
            Title = show.Title ?? string.Empty,
            PosterPath = show.PosterPath,
            Rating = show.Rating,
            ReleaseYear = show.ReleaseYear,
            GenreId = show.GenreId,
            Description = show.Description,
            MediaType = "tv",
            IsInWatchlist = show.IsInWatchlist,
            Action = PosterCardAction.ToggleWatchlist
        };

        public static MediaHomeViewModel ToMediaCard(this WatchlistViewModel movie) => new()
        {
            Id = movie.MovieId,
            Title = movie.Title ?? string.Empty,
            PosterPath = movie.PosterPath,
            Rating = movie.VoteAverage,
            ReleaseYear = movie.ReleaseYear,
            MediaType = "movie",
            IsInWatchlist = true,
            Action = PosterCardAction.RemoveFromWatchlist
        };

        public static MediaHomeViewModel ToMediaCard(this TvShowWatchlistViewModel show) => new()
        {
            Id = show.TvShowId,
            TmdbId = show.TmdbId,
            Title = show.Title ?? string.Empty,
            PosterPath = show.PosterPath,
            Rating = show.Rating,
            ReleaseYear = show.ReleaseYear,
            GenreId = show.GenreId,
            Description = show.Description,
            MediaType = "tv",
            IsInWatchlist = true,
            Action = PosterCardAction.RemoveFromWatchlist
        };
    }
}