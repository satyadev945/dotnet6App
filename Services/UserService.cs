using Microsoft.EntityFrameworkCore;
using SampleDotNet6App.Data;

namespace SampleDotNet6App.Services;

/// <summary>
/// User service implementation
/// </summary>
public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    /// <inheritdoc/>
    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    /// <inheritdoc/>
    public async Task<User> CreateUserAsync(User user)
    {
        user.CreatedAt = DateTime.UtcNow;
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
        existingUser.PasswordHash = user.PasswordHash;

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

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }
}
