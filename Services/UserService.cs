using Microsoft.EntityFrameworkCore;
using SampleDotNet6App.Data;
using SampleDotNet6App.Models;

namespace SampleDotNet6App.Services
{
    /// <summary>
    /// User service implementation
    /// </summary>
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserService> _logger;

        /// <summary>
        /// Constructor for UserService
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="logger">Logger instance</param>
        public UserService(ApplicationDbContext context, ILogger<UserService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Authenticates a user
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns>User or null</returns>
        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            _logger.LogInformation("Authenticating user: {Username}", username);
            
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                _logger.LogWarning("User not found: {Username}", username);
                return null;
            }

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                _logger.LogWarning("Invalid password for user: {Username}", username);
                return null;
            }

            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return user;
        }

        /// <summary>
        /// Gets all users
        /// </summary>
        /// <returns>List of users</returns>
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            _logger.LogInformation("Getting all users");
            return await _context.Users.ToListAsync();
        }

        /// <summary>
        /// Gets a user by ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User or null</returns>
        public async Task<User?> GetUserByIdAsync(int id)
        {
            _logger.LogInformation("Getting user with ID: {Id}", id);
            return await _context.Users.FindAsync(id);
        }

        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="user">User to create</param>
        /// <param name="password">User password</param>
        /// <returns>Created user</returns>
        public async Task<User> CreateUserAsync(User user, string password)
        {
            _logger.LogInformation("Creating new user: {Username}", user.Username);
            
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            user.CreatedAt = DateTime.UtcNow;
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            return user;
        }
    }
}
