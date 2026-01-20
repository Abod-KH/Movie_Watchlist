namespace Movie_Watchlist.Services
{
    public interface IFileService
    {
       
        Task<string> SaveImage(IFormFile imageFile);
    }
}
