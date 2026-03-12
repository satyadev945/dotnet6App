using Microsoft.EntityFrameworkCore;
using SampleDotNet6App.Data;
using SampleDotNet6App.DTOs;
using SampleDotNet6App.Models;

namespace SampleDotNet6App.Services
{
    /// <summary>
    /// Implementation of product service operations
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

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            _logger.LogInformation("Getting all products");
            var products = await _context.Products.ToListAsync();
            return products.Select(MapToDto);
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            _logger.LogInformation("Getting product with ID: {ProductId}", id);
            var product = await _context.Products.FindAsync(id);
            return product != null ? MapToDto(product) : null;
        }

        public async Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm)
        {
            _logger.LogInformation("Searching products with term: {SearchTerm}", searchTerm);
            var products = await _context.Products
                .Where(p => p.Name.Contains(searchTerm) || 
                           (p.Description != null && p.Description.Contains(searchTerm)) ||
                           (p.Category != null && p.Category.Contains(searchTerm)))
                .ToListAsync();
            return products.Select(MapToDto);
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto createDto)
        {
            _logger.LogInformation("Creating new product: {ProductName}", createDto.Name);
            var product = new Product
            {
                Name = createDto.Name,
                Description = createDto.Description,
                Price = createDto.Price,
                Category = createDto.Category,
                StockQuantity = createDto.StockQuantity,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Product created with ID: {ProductId}", product.Id);
            return MapToDto(product);
        }

        public async Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto updateDto)
        {
            _logger.LogInformation("Updating product with ID: {ProductId}", id);
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Product not found with ID: {ProductId}", id);
                return null;
            }

            if (updateDto.Name != null) product.Name = updateDto.Name;
            if (updateDto.Description != null) product.Description = updateDto.Description;
            if (updateDto.Price.HasValue) product.Price = updateDto.Price.Value;
            if (updateDto.Category != null) product.Category = updateDto.Category;
            if (updateDto.StockQuantity.HasValue) product.StockQuantity = updateDto.StockQuantity.Value;
            if (updateDto.IsActive.HasValue) product.IsActive = updateDto.IsActive.Value;
            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Product updated successfully: {ProductId}", id);
            return MapToDto(product);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            _logger.LogInformation("Deleting product with ID: {ProductId}", id);
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Product not found with ID: {ProductId}", id);
                return false;
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Product deleted successfully: {ProductId}", id);
            return true;
        }

        private static ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Category = product.Category,
                StockQuantity = product.StockQuantity,
                IsActive = product.IsActive,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };
        }
    }
}
