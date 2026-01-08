using SampleDotNet6App.Data;

namespace SampleDotNet6App.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
    }

    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await Task.FromResult(_context.Products.ToList());
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await Task.FromResult(_context.Products.FirstOrDefault(p => p.Id == id));
        }
    }
}
