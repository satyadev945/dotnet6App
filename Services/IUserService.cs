using SampleDotNet6App.Models;

namespace SampleDotNet6App.Services;

public interface IUserService
{
    Task<User?> GetUserByIdAsync(int id);
}
