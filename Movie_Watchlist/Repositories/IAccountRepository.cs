using Movie_Watchlist.Models;

namespace Movie_Watchlist.Repositories
{
    public interface IAccountRepository
    {
        Task<int> CreateUserAsync(User user);
        Task<User?> GetUserByEmailAsync(string email);
        Task AddUserToRoleAsync(int userId, string roleName);
    }
}
