using SampleDotNet6App.Models;

namespace SampleDotNet6App.Services;

public interface IUserService
{
    Task<User?> AuthenticateAsync(string username, string password);
    Task<User?> GetByIdAsync(int id);
    Task<User> CreateAsync(User user);
}
