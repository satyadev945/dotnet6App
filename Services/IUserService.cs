using SampleDotNet6App.DTOs;
using SampleDotNet6App.Models;

namespace SampleDotNet6App.Services;

/// <summary>
/// User service interface
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Authenticate user
    /// </summary>
    /// <param name="username">Username</param>
    /// <param name="password">Password</param>
    /// <returns>Login response or null</returns>
    Task<LoginResponse?> AuthenticateAsync(string username, string password);

    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="request">Registration request</param>
    /// <returns>Created user</returns>
    Task<User> RegisterAsync(RegisterRequest request);

    /// <summary>
    /// Get all users
    /// </summary>
    /// <returns>List of users</returns>
    Task<IEnumerable<User>> GetAllUsersAsync();

    /// <summary>
    /// Get user by ID
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>User or null</returns>
    Task<User?> GetUserByIdAsync(int id);

    /// <summary>
    /// Get user by username
    /// </summary>
    /// <param name="username">Username</param>
    /// <returns>User or null</returns>
    Task<User?> GetUserByUsernameAsync(string username);

    /// <summary>
    /// Update user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="user">Updated user data</param>
    /// <returns>Updated user or null</returns>
    Task<User?> UpdateUserAsync(int id, User user);

    /// <summary>
    /// Delete user
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>True if deleted, false otherwise</returns>
    Task<bool> DeleteUserAsync(int id);
}
