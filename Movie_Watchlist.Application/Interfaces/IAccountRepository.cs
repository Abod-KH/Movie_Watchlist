using Movie_Watchlist.Domain.Entities;

namespace Movie_Watchlist.Application.Interfaces
{
    public interface IAccountRepository
    {
        Task<int> CreateUserAsync(User user);
        Task<User?> GetUserByEmailAsync(string email);
        
    }
}
