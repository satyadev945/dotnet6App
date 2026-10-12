using SampleDotNet6App.Models;

namespace SampleDotNet6App.Services;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> AuthenticateAsync(string username, string password);
    Task<User> RegisterAsync(User user, string password);
    Task<bool> UserExistsAsync(string username, string email);
}
