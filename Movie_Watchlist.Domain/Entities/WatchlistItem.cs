using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace Movie_Watchlist.Domain.Entities
{
    [Table("WatchlistItem")]
    public class WatchlistItem
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = null!;

        [Required]
        public int MovieId { get; set; }
        public Movie? Movie { get; set; }

        public DateTime DateAdded { get; set; } = DateTime.Now;

        public bool IsWatched { get; set; } = false;

        public int? UserRating { get; set; }
    }
}
