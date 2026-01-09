using SampleDotNet6App.Models;

namespace SampleDotNet6App.Services;

/// <summary>
/// Interface for product service operations
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Gets all products
    /// </summary>
    /// <returns>A collection of products</returns>
    Task<IEnumerable<Product>> GetAllProductsAsync();

    /// <summary>
    /// Gets a product by identifier
    /// </summary>
    /// <param name="id">The product identifier</param>
    /// <returns>The product if found, null otherwise</returns>
    Task<Product?> GetProductByIdAsync(int id);

    /// <summary>
    /// Creates a new product
    /// </summary>
    /// <param name="product">The product to create</param>
    /// <returns>The created product</returns>
    Task<Product> CreateProductAsync(Product product);

    /// <summary>
    /// Updates an existing product
    /// </summary>
    /// <param name="product">The product to update</param>
    /// <returns>The updated product</returns>
    Task<Product> UpdateProductAsync(Product product);

    /// <summary>
    /// Deletes a product
    /// </summary>
    /// <param name="id">The product identifier</param>
    /// <returns>True if deleted, false otherwise</returns>
    Task<bool> DeleteProductAsync(int id);

    /// <summary>
    /// Gets products by category
    /// </summary>
    /// <param name="category">The category name</param>
    /// <returns>A collection of products in the category</returns>
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category);
}
