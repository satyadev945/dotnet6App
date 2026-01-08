using SampleDotNet6App.Data;

namespace SampleDotNet6App.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
    }

    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await Task.FromResult(_context.Users.ToList());
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await Task.FromResult(_context.Users.FirstOrDefault(u => u.Id == id));
        }
    }
}
