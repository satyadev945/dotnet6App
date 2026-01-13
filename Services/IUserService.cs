using SampleDotNet6App.Data;

namespace SampleDotNet6App.Services;

public interface IUserService
{
    Task<User?> AuthenticateAsync(string username, string password);
    Task<User?> GetUserByIdAsync(int id);
    Task<User> CreateUserAsync(User user, string password);
}
