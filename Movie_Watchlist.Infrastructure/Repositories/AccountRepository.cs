
namespace Movie_Watchlist.Infrastructure.Repositories
{
    public class AccountRepository : Repository, IAccountRepository
    {

        public AccountRepository(SqlConnectionFactory connectionFactory)
            : base(connectionFactory)
        {

        }

        public async Task<int> CreateUserAsync(User user)
        {
            return await ExecuteScalarAsync<int>("sp_User_Create", 
                ("@Username", user.Username), 
                ("@Email", user.Email), 
                ("@PasswordHash", user.PasswordHash));
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await ExecuteQuerySingleAsync<User>("sp_User_GetByEmail", ("@Email", email));
           
        }

        public async Task AddUserToRoleAsync(int userId, string roleName)
        {
            await ExecuteNonQueryAsync("sp_UserRole_Add", ("@UserId", userId), ("@RoleName", roleName));
        }

    }
}
