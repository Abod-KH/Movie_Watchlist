
using BCrypt.Net;
using Movie_Watchlist.Application.Interfaces;
using Movie_Watchlist.Application.Models;
using Movie_Watchlist.Domain.Entities;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;

    public AccountService(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<bool> RegisterUserAsync(UserRegister register)
    {
        
        var existingUser = await _accountRepository.GetUserByEmailAsync(register.Email);
        if (existingUser != null)
        {
            return false; 
        }

        
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(register.Password);

        
        var newUser = new User
        {
            Username = register.Username,
            Email = register.Email,
            PasswordHash = passwordHash,
            IsActive = true
        };

       
        int newUserId = await _accountRepository.CreateUserAsync(newUser);

        return newUserId > 0;
    }

    public async Task<User?> ValidateUserAsync(UserLogin login)
    {
        
        var user = await _accountRepository.GetUserByEmailAsync(login.Email);

        if (user == null || !user.IsActive)
        {
            return null; 
        }

        
        bool isValid = BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash);

        if (isValid)
        {
            return user;
        }

        return null; 
    }
}
