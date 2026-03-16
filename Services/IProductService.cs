using SampleDotNet6App.Models;

namespace SampleDotNet6App.Services
{
    /// <summary>
    /// Interface for product service
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// Gets all products
        /// </summary>
        /// <returns>List of products</returns>
        Task<IEnumerable<Product>> GetAllProductsAsync();

        /// <summary>
        /// Gets a product by ID
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Product or null</returns>
        Task<Product?> GetProductByIdAsync(int id);

        /// <summary>
        /// Creates a new product
        /// </summary>
        /// <param name="product">Product to create</param>
        /// <returns>Created product</returns>
        Task<Product> CreateProductAsync(Product product);

        /// <summary>
        /// Updates an existing product
        /// </summary>
        /// <param name="product">Product to update</param>
        /// <returns>Updated product</returns>
        Task<Product> UpdateProductAsync(Product product);

        /// <summary>
        /// Deletes a product
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>True if deleted</returns>
        Task<bool> DeleteProductAsync(int id);
    }
}
