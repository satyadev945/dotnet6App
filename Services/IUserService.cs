using SampleDotNet6App.Models;

namespace SampleDotNet6App.Services
{
    /// <summary>
    /// Interface for user service operations.
    /// </summary>
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<UserDto?> GetUserByUsernameAsync(string username);
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequest);
        Task<UserDto> RegisterAsync(RegisterUserDto registerDto);
        Task<bool> UpdateUserAsync(int id, UserDto userDto);
        Task<bool> DeactivateUserAsync(int id);
    }
}
