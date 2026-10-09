namespace SampleDotNet6App.Models
{
    /// <summary>
    /// Represents a product entity.
    /// </summary>
    public class Product
    {
        /// <summary>Gets or sets the unique identifier.</summary>
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
        public bool IsActive { get; set; } = true;

        /// <summary>Gets or sets the creation date.</summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>Gets or sets the last updated date.</summary>
        public DateTime? UpdatedAt { get; set; }
    }
}
