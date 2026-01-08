using SampleDotNet6App.Data;

namespace SampleDotNet6App.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
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
    }
}
