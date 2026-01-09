using SampleDotNet6App.Models;

namespace SampleDotNet6App.Services;

/// <summary>
/// Interface for user service operations
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Gets all users
    /// </summary>
    /// <returns>A collection of users</returns>
    Task<IEnumerable<User>> GetAllUsersAsync();

    /// <summary>
    /// Gets a user by identifier
    /// </summary>
    /// <param name="id">The user identifier</param>
    /// <returns>The user if found, null otherwise</returns>
    Task<User?> GetUserByIdAsync(int id);

    /// <summary>
    /// Gets a user by username
    /// </summary>
    /// <param name="username">The username</param>
    /// <returns>The user if found, null otherwise</returns>
    Task<User?> GetUserByUsernameAsync(string username);

    /// <summary>
    /// Gets a user by email
    /// </summary>
    /// <param name="email">The email address</param>
    /// <returns>The user if found, null otherwise</returns>
    Task<User?> GetUserByEmailAsync(string email);

    /// <summary>
    /// Creates a new user
    /// </summary>
    /// <param name="user">The user to create</param>
    /// <returns>The created user</returns>
    Task<User> CreateUserAsync(User user);

    /// <summary>
    /// Updates an existing user
    /// </summary>
    /// <param name="user">The user to update</param>
    /// <returns>The updated user</returns>
    Task<User> UpdateUserAsync(User user);

    /// <summary>
    /// Deletes a user
    /// </summary>
    /// <param name="id">The user identifier</param>
    /// <returns>True if deleted, false otherwise</returns>
    Task<bool> DeleteUserAsync(int id);

    /// <summary>
    /// Validates user credentials
    /// </summary>
    /// <param name="username">The username</param>
    /// <param name="password">The password</param>
    /// <returns>The user if credentials are valid, null otherwise</returns>
    Task<User?> ValidateCredentialsAsync(string username, string password);
}
