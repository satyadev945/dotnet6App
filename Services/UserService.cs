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

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Database context</param>
        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .Where(u => u.IsActive)
                .OrderBy(u => u.Username)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
        }

        /// <inheritdoc/>
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);
        }

        /// <inheritdoc/>
        public async Task<User> CreateUserAsync(User user)
        {
            user.CreatedDate = DateTime.UtcNow;
            user.IsActive = true;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        /// <inheritdoc/>
        public async Task<User?> UpdateUserAsync(int id, User user)
        {
            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
            {
                return null;
            }

            existingUser.Username = user.Username;
            existingUser.Email = user.Email;
            
            // Only update password if provided
            if (!string.IsNullOrEmpty(user.PasswordHash))
            {
                existingUser.PasswordHash = user.PasswordHash;
            }

            await _context.SaveChangesAsync();

            return existingUser;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return false;
            }

            // Soft delete
            user.IsActive = false;

            await _context.SaveChangesAsync();

            return true;
        }

        /// <inheritdoc/>
        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            var user = await GetUserByUsernameAsync(username);
            if (user == null)
            {
                return null;
            }

            // In a real application, you would verify the password hash here
            // For this sample, we'll just check if the user exists
            // TODO: Implement proper password verification

            user.LastLoginDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return user;
        }
    }
}
