using SampleDotNet6App.Data;

namespace SampleDotNet6App.Services;

public interface IProductService
{
    Task<IEnumerable<Product>> GetAllProductsAsync();
    Task<Product?> GetProductByIdAsync(int id);
    Task<Product> CreateProductAsync(Product product);
    Task<Product?> UpdateProductAsync(int id, Product product);
    Task<bool> DeleteProductAsync(int id);
}

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;

    public ProductService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        return await Task.FromResult(_context.Products.ToList());
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await Task.FromResult(_context.Products.FirstOrDefault(p => p.Id == id));
    }

    public async Task<Product> CreateProductAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<Product?> UpdateProductAsync(int id, Product product)
    {
        var existing = await GetProductByIdAsync(id);
        if (existing == null) return null;

        existing.Name = product.Name;
        existing.Price = product.Price;
        existing.Description = product.Description;
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await GetProductByIdAsync(id);
        if (product == null) return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return true;
    }
}
