using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Movie_Watchlist.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Movie_Watchlist.Application.ViewModels
{
    public class MovieFormModel
    {
        public int Id { get; set; }

        [Required]
        public string? Title { get; set; }

        [Required]
        public int ReleaseYear { get; set; }

        [Required]
        public int GenreId { get; set; }

       
        public IFormFile? ImageFile { get; set; }

        [ValidateNever]
        public IEnumerable<Genre>? GenreList { get; set; }
    }
}
