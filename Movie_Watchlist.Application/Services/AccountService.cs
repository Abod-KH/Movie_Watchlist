
using BCrypt.Net;
using Microsoft.Extensions.Logging;
using Movie_Watchlist.Application.Interfaces;
using Movie_Watchlist.Application.Models;
using Movie_Watchlist.Domain.Entities;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly ILogger<AccountService> _logger;


    public AccountService(IAccountRepository accountRepository, ILogger<AccountService> logger)
    {
        _accountRepository = accountRepository;
        _logger = logger;
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


        try
        {
            int newUserId = await _accountRepository.CreateUserAsync(newUser);
            return newUserId > 0;
        }
        catch (Exception ex)
        {
            

            _logger.LogError(ex, "Error registering user");

            return false;
        }
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

    public async Task<User?> GetOrCreateExternalUserAsync(string email, string name)
    {
        var existingUser = await _accountRepository.GetUserByEmailAsync(email);
        if (existingUser != null)
        {
            return existingUser;
        }

        string username = string.IsNullOrWhiteSpace(name) ? email.Split('@')[0] : name;
        if (username.Length > 16) username = username.Substring(0, 16);

        var newUser = new User
        {
            Username = username,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString()),
            IsActive = true
        };

        try
        {
            int newUserId = await _accountRepository.CreateUserAsync(newUser);
            if (newUserId > 0)
            {
                return await _accountRepository.GetUserByEmailAsync(email);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating external user for email {Email}", email);
        }

        return null;
    }
}
