using Microsoft.EntityFrameworkCore;
using SampleDotNet6App.Data;
using SampleDotNet6App.Models;

namespace SampleDotNet6App.Services
{
    /// <summary>
    /// Implementation of product business logic operations.
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductService> _logger;

        public ProductService(ApplicationDbContext context, ILogger<ProductService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category)
        {
            return await _context.Products
                .Where(p => p.Category == category && p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            product.CreatedAt = DateTime.UtcNow;
            product.IsActive = true;

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Product created: {ProductId} - {ProductName}", product.Id, product.Name);
            return product;
        }

        public async Task<Product?> UpdateProductAsync(int id, Product product)
        {
            var existing = await _context.Products.FindAsync(id);
            if (existing == null) return null;

            existing.Name = product.Name;
            existing.Description = product.Description;
            existing.Price = product.Price;
            existing.Category = product.Category;
            existing.StockQuantity = product.StockQuantity;
            existing.ImageUrl = product.ImageUrl;
            existing.SKU = product.SKU;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Product updated: {ProductId} - {ProductName}", id, existing.Name);
            return existing;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            // Soft delete
            product.IsActive = false;
            product.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Product deleted (soft): {ProductId}", id);
            return true;
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
        {
            var term = searchTerm.ToLower();
            return await _context.Products
                .Where(p => p.IsActive &&
                    (p.Name.ToLower().Contains(term) ||
                     (p.Description != null && p.Description.ToLower().Contains(term)) ||
                     (p.Category != null && p.Category.ToLower().Contains(term)) ||
                     (p.SKU != null && p.SKU.ToLower().Contains(term))))
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<bool> UpdateStockAsync(int id, int quantity)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            product.StockQuantity = quantity;
            product.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Stock updated for product {ProductId}: {Quantity}", id, quantity);
            return true;
        }
    }
}
