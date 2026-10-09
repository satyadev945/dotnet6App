using SampleDotNet6App.DTOs;
using SampleDotNet6App.Models;

namespace SampleDotNet6App.Services
{
    /// <summary>
    /// Defines the contract for product-related business operations.
    /// </summary>
    public interface IProductService
    {
        /// <summary>Gets all products.</summary>
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();

        /// <summary>Gets a product by its identifier.</summary>
        Task<ProductDto?> GetProductByIdAsync(int id);

        /// <summary>Creates a new product.</summary>
        Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto);

        /// <summary>Updates an existing product.</summary>
        Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto updateProductDto);

        /// <summary>Deletes a product by its identifier.</summary>
        Task<bool> DeleteProductAsync(int id);

        /// <summary>Gets products by category.</summary>
        Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(string category);
    }
}
