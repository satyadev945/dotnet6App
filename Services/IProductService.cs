using SampleDotNet6App.Models;

namespace SampleDotNet6App.Services;

/// <summary>
/// Product service interface
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Get all products
    /// </summary>
    /// <returns>List of products</returns>
    Task<IEnumerable<Product>> GetAllProductsAsync();

    /// <summary>
    /// Get product by ID
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>Product or null</returns>
    Task<Product?> GetProductByIdAsync(int id);

    /// <summary>
    /// Search products by name or description
    /// </summary>
    /// <param name="searchTerm">Search term</param>
    /// <returns>List of matching products</returns>
    Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);

    /// <summary>
    /// Create a new product
    /// </summary>
    /// <param name="product">Product to create</param>
    /// <returns>Created product</returns>
    Task<Product> CreateProductAsync(Product product);

    /// <summary>
    /// Update an existing product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="product">Updated product data</param>
    /// <returns>Updated product or null</returns>
    Task<Product?> UpdateProductAsync(int id, Product product);

    /// <summary>
    /// Delete a product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>True if deleted, false otherwise</returns>
    Task<bool> DeleteProductAsync(int id);

    /// <summary>
    /// Get products by category
    /// </summary>
    /// <param name="category">Category name</param>
    /// <returns>List of products in category</returns>
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category);
}
