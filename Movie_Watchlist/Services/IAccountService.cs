using Movie_Watchlist.Models;

namespace Movie_Watchlist.Services
{
    public interface IAccountService
    {
        Task<bool> RegisterUserAsync(UserRegister register);
        Task<User?> ValidateUserAsync(UserLogin login);
    }
}
