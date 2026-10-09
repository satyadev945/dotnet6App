using SampleDotNet6App.DTOs;

namespace SampleDotNet6App.Services
{
    /// <summary>
    /// Defines the contract for user-related business operations.
    /// </summary>
    public interface IUserService
    {
        /// <summary>Gets all users.</summary>
        Task<IEnumerable<UserDto>> GetAllUsersAsync();

        /// <summary>Gets a user by their identifier.</summary>
        Task<UserDto?> GetUserByIdAsync(int id);

        /// <summary>Gets a user by their username.</summary>
        Task<UserDto?> GetUserByUsernameAsync(string username);

        /// <summary>Creates a new user.</summary>
        Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);

        /// <summary>Authenticates a user and returns a JWT token response.</summary>
        Task<LoginResponseDto?> AuthenticateAsync(LoginRequestDto loginRequest);

        /// <summary>Deletes a user by their identifier.</summary>
        Task<bool> DeleteUserAsync(int id);
    }
}
