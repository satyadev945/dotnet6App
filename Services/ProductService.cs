using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SampleDotNet6App.Data;
using SampleDotNet6App.DTOs;
using SampleDotNet6App.Models;

namespace SampleDotNet6App.Services
{
    /// <summary>
    /// Provides product-related business operations.
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductService> _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="ProductService"/>.
        /// </summary>
        public ProductService(ApplicationDbContext context, IMapper mapper, ILogger<ProductService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _context.Products
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        /// <inheritdoc />
        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            return product == null ? null : _mapper.Map<ProductDto>(product);
        }

        /// <inheritdoc />
        public async Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto)
        {
            var product = _mapper.Map<Product>(createProductDto);
            product.CreatedAt = DateTime.UtcNow;
            product.IsActive = true;

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created product with ID {ProductId}", product.Id);
            return _mapper.Map<ProductDto>(product);
        }

        /// <inheritdoc />
        public async Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto updateProductDto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return null;
            }

            if (updateProductDto.Name != null) product.Name = updateProductDto.Name;
            if (updateProductDto.Description != null) product.Description = updateProductDto.Description;
            if (updateProductDto.Price.HasValue) product.Price = updateProductDto.Price.Value;
            if (updateProductDto.StockQuantity.HasValue) product.StockQuantity = updateProductDto.StockQuantity.Value;
            if (updateProductDto.Category != null) product.Category = updateProductDto.Category;
            if (updateProductDto.IsActive.HasValue) product.IsActive = updateProductDto.IsActive.Value;
            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Updated product with ID {ProductId}", product.Id);
            return _mapper.Map<ProductDto>(product);
        }

        /// <inheritdoc />
        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return false;
            }

            product.IsActive = false;
            product.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Soft-deleted product with ID {ProductId}", id);
            return true;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(string category)
        {
            var products = await _context.Products
                .Where(p => p.IsActive && p.Category == category)
                .OrderBy(p => p.Name)
                .ToListAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }
    }
}
