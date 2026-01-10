using SampleDotNet6App.Data;

namespace SampleDotNet6App.Services;

/// <summary>
/// User service interface
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Gets all users
    /// </summary>
    Task<IEnumerable<User>> GetAllUsersAsync();

    /// <summary>
    /// Gets a user by ID
    /// </summary>
    Task<User?> GetUserByIdAsync(int id);

    /// <summary>
    /// Gets a user by username
    /// </summary>
    Task<User?> GetUserByUsernameAsync(string username);

    /// <summary>
    /// Creates a new user
    /// </summary>
    Task<User> CreateUserAsync(User user);

    /// <summary>
    /// Updates an existing user
    /// </summary>
    Task<User?> UpdateUserAsync(int id, User user);

    /// <summary>
    /// Deletes a user
    /// </summary>
    Task<bool> DeleteUserAsync(int id);
}
