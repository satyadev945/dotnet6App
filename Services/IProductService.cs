using SampleDotNet6App.DTOs;

namespace SampleDotNet6App.Services
{
    /// <summary>
    /// Interface for product service operations
    /// </summary>
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<ProductDto?> GetProductByIdAsync(int id);
        Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm);
        Task<ProductDto> CreateProductAsync(CreateProductDto createDto);
        Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto updateDto);
        Task<bool> DeleteProductAsync(int id);
    }
}
