using SampleDotNet6App.DTOs;

namespace SampleDotNet6App.Services
{
    /// <summary>
    /// Interface for user service operations
    /// </summary>
    public interface IUserService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginDto);
        Task<UserDto> RegisterAsync(RegisterRequestDto registerDto);
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserProfileAsync(string username);
    }
}
