using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Movie_Watchlist.Domain.Entities
{
  
    public class Genre
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(40)]
        public string Name { get; set; } 
        public List<Movie> Movies { get; set; } = new();
    }
}
