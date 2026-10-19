using SampleDotNet6App.Models;

namespace SampleDotNet6App.Services
{
    public interface IUserService
    {
        Task<LoginResponse?> LoginAsync(LoginRequest request);
        Task<UserDto?> RegisterAsync(RegisterRequest request);
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<UserDto?> GetUserByUsernameAsync(string username);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
    }
}
