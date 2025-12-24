using SampleDotNet6App.Models;

namespace SampleDotNet6App.Services;

/// <summary>
/// Defines the contract for product-related business operations.
/// </summary>
public interface IProductService
{
    Task<IEnumerable<Product>> GetAllProductsAsync();
    Task<Product?> GetProductByIdAsync(int id);
    Task<Product> CreateProductAsync(Product product);
    Task<Product?> UpdateProductAsync(Product product);
    Task<bool> DeleteProductAsync(int id);
}
