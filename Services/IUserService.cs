using SampleDotNet6App.Models;

namespace SampleDotNet6App.Services
{
    /// <summary>
    /// Interface for user service
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Authenticates a user
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns>User or null</returns>
        Task<User?> AuthenticateAsync(string username, string password);

        /// <summary>
        /// Gets all users
        /// </summary>
        /// <returns>List of users</returns>
        Task<IEnumerable<User>> GetAllUsersAsync();

        /// <summary>
        /// Gets a user by ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User or null</returns>
        Task<User?> GetUserByIdAsync(int id);

        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="user">User to create</param>
        /// <param name="password">User password</param>
        /// <returns>Created user</returns>
        Task<User> CreateUserAsync(User user, string password);
    }
}
