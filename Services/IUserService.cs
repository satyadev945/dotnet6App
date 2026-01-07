using SampleDotNet6App.Data;

namespace SampleDotNet6App.Services;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(int id);
    Task<User?> GetUserByUsernameAsync(string username);
    Task<User> CreateUserAsync(User user);
}

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await Task.FromResult(_context.Users.ToList());
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await Task.FromResult(_context.Users.FirstOrDefault(u => u.Id == id));
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await Task.FromResult(_context.Users.FirstOrDefault(u => u.Username == username));
    }

    public async Task<User> CreateUserAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }
}
