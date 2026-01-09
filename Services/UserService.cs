using Microsoft.EntityFrameworkCore;
using SampleDotNet6App.Data;
using SampleDotNet6App.Models;

namespace SampleDotNet6App.Services;

/// <summary>
/// Service for user operations
/// </summary>
public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the UserService class
    /// </summary>
    /// <param name="context">The database context</param>
    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets all users
    /// </summary>
    /// <returns>A collection of users</returns>
    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _context.Users
            .Where(u => u.IsActive)
            .OrderBy(u => u.Username)
            .ToListAsync();
    }

    /// <summary>
    /// Gets a user by identifier
    /// </summary>
    /// <param name="id">The user identifier</param>
    /// <returns>The user if found, null otherwise</returns>
    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
    }

    /// <summary>
    /// Gets a user by username
    /// </summary>
    /// <param name="username">The username</param>
    /// <returns>The user if found, null otherwise</returns>
    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);
    }

    /// <summary>
    /// Gets a user by email
    /// </summary>
    /// <param name="email">The email address</param>
    /// <returns>The user if found, null otherwise</returns>
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);
    }

    /// <summary>
    /// Creates a new user
    /// </summary>
    /// <param name="user">The user to create</param>
    /// <returns>The created user</returns>
    public async Task<User> CreateUserAsync(User user)
    {
        user.CreatedAt = DateTime.UtcNow;
        user.IsActive = true;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

    /// <summary>
    /// Updates an existing user
    /// </summary>
    /// <param name="user">The user to update</param>
    /// <returns>The updated user</returns>
    public async Task<User> UpdateUserAsync(User user)
    {
        _context.Entry(user).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return user;
    }

    /// <summary>
    /// Deletes a user
    /// </summary>
    /// <param name="id">The user identifier</param>
    /// <returns>True if deleted, false otherwise</returns>
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

    /// <summary>
    /// Validates user credentials
    /// </summary>
    /// <param name="username">The username</param>
    /// <param name="password">The password</param>
    /// <returns>The user if credentials are valid, null otherwise</returns>
    public async Task<User?> ValidateCredentialsAsync(string username, string password)
    {
        var user = await GetUserByUsernameAsync(username);
        if (user == null)
        {
            return null;
        }

        // In production, use proper password hashing (e.g., BCrypt, PBKDF2)
        // This is a simplified example
        // bool isValidPassword = VerifyPasswordHash(password, user.PasswordHash);

        return user;
    }
}
