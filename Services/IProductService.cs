using SampleDotNet6App.Data;

namespace SampleDotNet6App.Services;

/// <summary>
/// Product service interface
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Gets all products
    /// </summary>
    Task<IEnumerable<Product>> GetAllProductsAsync();

    /// <summary>
    /// Gets a product by ID
    /// </summary>
    Task<Product?> GetProductByIdAsync(int id);

    /// <summary>
    /// Creates a new product
    /// </summary>
    Task<Product> CreateProductAsync(Product product);

    /// <summary>
    /// Updates an existing product
    /// </summary>
    Task<Product?> UpdateProductAsync(int id, Product product);

    /// <summary>
    /// Deletes a product
    /// </summary>
    Task<bool> DeleteProductAsync(int id);
}
