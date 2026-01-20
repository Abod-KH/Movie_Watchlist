using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Movie_Watchlist.Models.DTOs
{
    public class MovieFormModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public int ReleaseYear { get; set; }

        [Required]
        public int GenreId { get; set; }

       
        public IFormFile? ImageFile { get; set; }

        [ValidateNever]
        public IEnumerable<Genre> GenreList { get; set; }
    }
}
