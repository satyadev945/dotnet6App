using System.ComponentModel.DataAnnotations;

namespace SampleDotNet6App.Models;

/// <summary>
/// Product entity representing items in the catalog
/// </summary>
public class Product
{
    /// <summary>
    /// Unique identifier for the product
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Product name
    /// </summary>
    [Required(ErrorMessage = "Product name is required")]
    [StringLength(200, ErrorMessage = "Product name must not exceed 200 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Product description
    /// </summary>
    [StringLength(1000, ErrorMessage = "Description must not exceed 1000 characters")]
    public string? Description { get; set; }

    /// <summary>
    /// Product price
    /// </summary>
    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, 999999.99, ErrorMessage = "Price must be between 0.01 and 999999.99")]
    public decimal Price { get; set; }

    /// <summary>
    /// Product category
    /// </summary>
    [Required(ErrorMessage = "Category is required")]
    [StringLength(100, ErrorMessage = "Category must not exceed 100 characters")]
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Stock quantity
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be non-negative")]
    public int StockQuantity { get; set; }

    /// <summary>
    /// Product SKU (Stock Keeping Unit)
    /// </summary>
    [StringLength(50, ErrorMessage = "SKU must not exceed 50 characters")]
    public string? Sku { get; set; }

    /// <summary>
    /// Indicates if the product is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Date when the product was created
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date when the product was last updated
    /// </summary>
    public DateTime? LastUpdatedDate { get; set; }
}
