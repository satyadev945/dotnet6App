using SampleDotNet6App.DTOs;
using SampleDotNet6App.Models;

namespace SampleDotNet6App.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<UserDto?> GetUserByUsernameAsync(string username);
        Task<UserDto> RegisterAsync(RegisterUserDto registerDto);
        Task<LoginResponseDto?> LoginAsync(LoginDto loginDto);
    }
}
