using SampleDotNet6App.Models;

namespace SampleDotNet6App.Services
{
    /// <summary>
    /// Interface for product service operations.
    /// </summary>
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<ProductDto?> GetProductByIdAsync(int id);
        Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm);
        Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(string category);
        Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto);
        Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto updateProductDto);
        Task<bool> DeleteProductAsync(int id);
    }
}
