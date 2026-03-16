namespace SampleDotNet6App.Models
{
    /// <summary>
    /// Product entity
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Product ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Product name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Product description
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Product price
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Product category
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// Stock quantity
        /// </summary>
        public int StockQuantity { get; set; }

        /// <summary>
        /// Created date
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Last updated date
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}
