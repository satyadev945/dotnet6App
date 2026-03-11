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
        /// Stock quantity
        /// </summary>
        public int Stock { get; set; }

        /// <summary>
        /// Is product active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Created date
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Last modified date
        /// </summary>
        public DateTime? ModifiedDate { get; set; }
    }
}
