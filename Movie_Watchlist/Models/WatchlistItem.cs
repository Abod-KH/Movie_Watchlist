using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Identity;
namespace Movie_Watchlist.Models
{
    [Table("WatchlistItem")]
    public class WatchlistItem
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } // Links to Identity User

        [Required]
        public int MovieId { get; set; }
        public Movie Movie { get; set; }

        public DateTime DateAdded { get; set; } = DateTime.Now;

        public bool IsWatched { get; set; } = false; 

        public int? UserRating { get; set; } 
    }
}
