using SampleDotNet6App.Data;

namespace SampleDotNet6App.Services;

public interface IUserService
{
    Task<User?> GetUserByUsernameAsync(string username);
    Task<User?> GetUserByIdAsync(int id);
    Task<User> CreateUserAsync(User user);
}
