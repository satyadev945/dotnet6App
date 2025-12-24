using SampleDotNet6App.Models;

namespace SampleDotNet6App.Services;

/// <summary>
/// Defines the contract for user-related business operations.
/// </summary>
public interface IUserService
{
    Task<User?> GetUserByIdAsync(int id);
    Task<User?> GetUserByUsernameAsync(string username);
    Task<User> CreateUserAsync(User user);
}
