namespace Movie_Watchlist.Models.DTOs
{
    public class MovieDisplayModel
    {
 
        public IEnumerable<MovieHomeViewModel> Movies { get; set; }

       
        public IEnumerable<Genre> Genres { get; set; }

       
        public string STerm { get; set; } = "";
        public int GenreId { get; set; } = 0;
    }
}
