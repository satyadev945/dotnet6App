using Microsoft.EntityFrameworkCore;
using SampleDotNet6App.Data;
using SampleDotNet6App.Models;

namespace SampleDotNet6App.Services
{
    /// <summary>
    /// Product service implementation
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductService> _logger;

        /// <summary>
        /// Constructor for ProductService
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="logger">Logger instance</param>
        public ProductService(ApplicationDbContext context, ILogger<ProductService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Gets all products
        /// </summary>
        /// <returns>List of products</returns>
        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            _logger.LogInformation("Getting all products");
            return await _context.Products.ToListAsync();
        }

        /// <summary>
        /// Gets a product by ID
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Product or null</returns>
        public async Task<Product?> GetProductByIdAsync(int id)
        {
            _logger.LogInformation("Getting product with ID: {Id}", id);
            return await _context.Products.FindAsync(id);
        }

        /// <summary>
        /// Creates a new product
        /// </summary>
        /// <param name="product">Product to create</param>
        /// <returns>Created product</returns>
        public async Task<Product> CreateProductAsync(Product product)
        {
            _logger.LogInformation("Creating new product: {Name}", product.Name);
            product.CreatedAt = DateTime.UtcNow;
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        /// <summary>
        /// Updates an existing product
        /// </summary>
        /// <param name="product">Product to update</param>
        /// <returns>Updated product</returns>
        public async Task<Product> UpdateProductAsync(Product product)
        {
            _logger.LogInformation("Updating product with ID: {Id}", product.Id);
            product.UpdatedAt = DateTime.UtcNow;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }

        /// <summary>
        /// Deletes a product
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>True if deleted</returns>
        public async Task<bool> DeleteProductAsync(int id)
        {
            _logger.LogInformation("Deleting product with ID: {Id}", id);
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return false;
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
