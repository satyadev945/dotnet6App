using SampleDotNet6App.Models;

namespace SampleDotNet6App.Services
{
    /// <summary>
    /// User service interface
    /// </summary>
    public interface IUserService
    {
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
        /// Create a new user
        /// </summary>
        /// <param name="user">User to create</param>
        /// <returns>Created user</returns>
        Task<User> CreateUserAsync(User user);

        /// <summary>
        /// Update an existing user
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="user">Updated user data</param>
        /// <returns>Updated user or null</returns>
        Task<User?> UpdateUserAsync(int id, User user);

        /// <summary>
        /// Delete a user
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>True if deleted, false otherwise</returns>
        Task<bool> DeleteUserAsync(int id);

        /// <summary>
        /// Authenticate user
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns>User if authenticated, null otherwise</returns>
        Task<User?> AuthenticateAsync(string username, string password);
    }
}
