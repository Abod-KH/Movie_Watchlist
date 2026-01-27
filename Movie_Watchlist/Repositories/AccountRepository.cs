using Microsoft.Data.SqlClient;
using Movie_Watchlist.Data;
using Movie_Watchlist.Models;
using System.Data;

namespace Movie_Watchlist.Repositories
{
    public class AccountRepository : Repository, IAccountRepository
    {

        public AccountRepository(SqlConnectionFactory connectionFactory)
            : base(connectionFactory) 
        {
            
        }

        public async Task<int> CreateUserAsync(User user)
        {
            using var connection = _connectionFactory.CreateConnection();
            var command = new SqlCommand("sp_User_Create", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@Username", user.Username);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);

            await connection.OpenAsync();

           
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            using var connection = _connectionFactory.CreateConnection();
            var command = new SqlCommand("sp_User_GetByEmail", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Email", email);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var user = MapReaderToObject<User>(reader);

            
                if (reader["RoleName"] != DBNull.Value)
                {
                    user.RoleName = reader["RoleName"].ToString();
                }

                return user;
            }

            return null;
        }

        public async Task AddUserToRoleAsync(int userId, string roleName)
        {
            using var connection = _connectionFactory.CreateConnection();
            var command = new SqlCommand("sp_UserRole_Add", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@RoleName", roleName);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

    }
}
