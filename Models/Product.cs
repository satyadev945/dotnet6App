using System.ComponentModel.DataAnnotations;

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
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Product description
        /// </summary>
        [StringLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Product price
        /// </summary>
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        /// <summary>
        /// Product category
        /// </summary>
        [StringLength(50)]
        public string? Category { get; set; }

        /// <summary>
        /// Stock quantity
        /// </summary>
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        /// <summary>
        /// Is product active
        /// </summary>
        public bool IsActive { get; set; } = true;

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
