using Movie_Watchlist.Application.Models;
using Movie_Watchlist.Domain.Entities;

namespace Movie_Watchlist.Application.Interfaces
{
    public interface IAccountService
    {
        Task<bool> RegisterUserAsync(UserRegister register);
        Task<User?> ValidateUserAsync(UserLogin login);
    }
}
