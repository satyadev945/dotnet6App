using Microsoft.EntityFrameworkCore;
using SampleDotNet6App.Data;
using SampleDotNet6App.Models;

namespace SampleDotNet6App.Services;

/// <summary>
/// Product service implementation
/// </summary>
public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">Database context</param>
    public ProductService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        return await _context.Products
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await GetAllProductsAsync();
        }

        var lowerSearchTerm = searchTerm.ToLower();
        return await _context.Products
            .Where(p => p.IsActive &&
                       (p.Name.ToLower().Contains(lowerSearchTerm) ||
                        (p.Description != null && p.Description.ToLower().Contains(lowerSearchTerm))))
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<Product> CreateProductAsync(Product product)
    {
        product.CreatedDate = DateTime.UtcNow;
        product.IsActive = true;

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return product;
    }

    /// <inheritdoc/>
    public async Task<Product?> UpdateProductAsync(int id, Product product)
    {
        var existingProduct = await _context.Products.FindAsync(id);
        if (existingProduct == null)
        {
            return null;
        }

        existingProduct.Name = product.Name;
        existingProduct.Description = product.Description;
        existingProduct.Price = product.Price;
        existingProduct.Category = product.Category;
        existingProduct.StockQuantity = product.StockQuantity;
        existingProduct.Sku = product.Sku;
        existingProduct.IsActive = product.IsActive;
        existingProduct.LastUpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return existingProduct;
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return false;
        }

        // Soft delete
        product.IsActive = false;
        product.LastUpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category)
    {
        return await _context.Products
            .Where(p => p.IsActive && p.Category.ToLower() == category.ToLower())
            .OrderBy(p => p.Name)
            .ToListAsync();
    }
}
