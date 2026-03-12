namespace SampleDotNet6App.DTOs
{
    /// <summary>
    /// Product DTO
    /// </summary>
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? Category { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// Create Product DTO
    /// </summary>
    public class CreateProductDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? Category { get; set; }
        public int StockQuantity { get; set; }
    }

    /// <summary>
    /// Update Product DTO
    /// </summary>
    public class UpdateProductDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public string? Category { get; set; }
        public int? StockQuantity { get; set; }
        public bool? IsActive { get; set; }
    }
}
