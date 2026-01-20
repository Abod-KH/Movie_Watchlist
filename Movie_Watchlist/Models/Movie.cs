using Humanizer.Localisation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Movie_Watchlist.Models
{
    [Table("Movie")]
    public class Movie
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }
        [MaxLength(1000)]
        public string? Description { get; set; }

        public string? PosterPath { get; set; }

        public int ReleaseYear { get; set; }

        public int GenreId { get; set; }
        public Genre Genre { get; set; }

        public List<WatchlistItem> WatchlistItem { get; set; } = new List<WatchlistItem>();
    }
}
