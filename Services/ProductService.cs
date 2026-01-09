using Microsoft.EntityFrameworkCore;
using SampleDotNet6App.Data;
using SampleDotNet6App.Models;

namespace SampleDotNet6App.Services;

/// <summary>
/// Service for product operations
/// </summary>
public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the ProductService class
    /// </summary>
    /// <param name="context">The database context</param>
    public ProductService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets all products
    /// </summary>
    /// <returns>A collection of products</returns>
    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        return await _context.Products
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Gets a product by identifier
    /// </summary>
    /// <param name="id">The product identifier</param>
    /// <returns>The product if found, null otherwise</returns>
    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
    }

    /// <summary>
    /// Creates a new product
    /// </summary>
    /// <param name="product">The product to create</param>
    /// <returns>The created product</returns>
    public async Task<Product> CreateProductAsync(Product product)
    {
        product.CreatedAt = DateTime.UtcNow;
        product.IsActive = true;

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return product;
    }

    /// <summary>
    /// Updates an existing product
    /// </summary>
    /// <param name="product">The product to update</param>
    /// <returns>The updated product</returns>
    public async Task<Product> UpdateProductAsync(Product product)
    {
        _context.Entry(product).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return product;
    }

    /// <summary>
    /// Deletes a product
    /// </summary>
    /// <param name="id">The product identifier</param>
    /// <returns>True if deleted, false otherwise</returns>
    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return false;
        }

        // Soft delete
        product.IsActive = false;
        await _context.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Gets products by category
    /// </summary>
    /// <param name="category">The category name</param>
    /// <returns>A collection of products in the category</returns>
    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category)
    {
        return await _context.Products
            .Where(p => p.Category == category && p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }
}
