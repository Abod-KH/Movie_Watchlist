using Microsoft.AspNetCore.Http;

namespace Movie_Watchlist.Application.Interfaces
{
    public interface IFileService
    {
       
        Task<string> SaveImage(IFormFile imageFile);
    }
}
