using SampleDotNet6App.Models;

namespace SampleDotNet6App.DTOs
{
    /// <summary>
    /// Data transfer object for Product responses.
    /// </summary>
    public class ProductDto
    {
        /// <summary>Gets or sets the product ID.</summary>
        public int Id { get; set; }

        /// <summary>Gets or sets the product name.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the product description.</summary>
        public string? Description { get; set; }

        /// <summary>Gets or sets the product price.</summary>
        public decimal Price { get; set; }

        /// <summary>Gets or sets the stock quantity.</summary>
        public int StockQuantity { get; set; }

        /// <summary>Gets or sets the product category.</summary>
        public string? Category { get; set; }

        /// <summary>Gets or sets whether the product is active.</summary>
        public bool IsActive { get; set; }

        /// <summary>Gets or sets the creation date.</summary>
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// Data transfer object for creating a product.
    /// </summary>
    public class CreateProductDto
    {
        /// <summary>Gets or sets the product name.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the product description.</summary>
        public string? Description { get; set; }

        /// <summary>Gets or sets the product price.</summary>
        public decimal Price { get; set; }

        /// <summary>Gets or sets the stock quantity.</summary>
        public int StockQuantity { get; set; }

        /// <summary>Gets or sets the product category.</summary>
        public string? Category { get; set; }
    }

    /// <summary>
    /// Data transfer object for updating a product.
    /// </summary>
    public class UpdateProductDto
    {
        /// <summary>Gets or sets the product name.</summary>
        public string? Name { get; set; }

        /// <summary>Gets or sets the product description.</summary>
        public string? Description { get; set; }

        /// <summary>Gets or sets the product price.</summary>
        public decimal? Price { get; set; }

        /// <summary>Gets or sets the stock quantity.</summary>
        public int? StockQuantity { get; set; }

        /// <summary>Gets or sets the product category.</summary>
        public string? Category { get; set; }

        /// <summary>Gets or sets whether the product is active.</summary>
        public bool? IsActive { get; set; }
    }
}
