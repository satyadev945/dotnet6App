using SampleDotNet6App.Data;

namespace SampleDotNet6App.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
    }

    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await Task.FromResult(_context.Products.ToList());
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await Task.FromResult(_context.Products.FirstOrDefault(p => p.Id == id));
        }
    }
}
