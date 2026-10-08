using SampleDotNet6App.Models;

namespace SampleDotNet6App.Services
{
    /// <summary>
    /// Interface for user business logic and authentication operations.
    /// </summary>
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User> RegisterAsync(User user, string password);
        Task<(User? User, string? Token)> AuthenticateAsync(string username, string password);
        Task<User?> UpdateUserAsync(int id, User user);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> ChangePasswordAsync(int id, string currentPassword, string newPassword);
        string GenerateJwtToken(User user);
    }
}
