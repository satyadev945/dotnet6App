using SampleDotNet6App.Models.DTOs;

namespace SampleDotNet6App.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto?> GetByIdAsync(int id);
        Task<UserDto?> GetByUsernameAsync(string username);
        Task<UserDto> RegisterAsync(RegisterUserDto dto);
        Task<string?> LoginAsync(LoginDto dto);
        Task<UserDto?> GetProfileAsync(string username);
    }
}
