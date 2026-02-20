namespace SampleDotNet6App.DTOs;

/// <summary>
/// Product data transfer object
/// </summary>
public class ProductDto
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
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Stock quantity
    /// </summary>
    public int StockQuantity { get; set; }

    /// <summary>
    /// Product SKU
    /// </summary>
    public string? Sku { get; set; }

    /// <summary>
    /// Is product active
    /// </summary>
    public bool IsActive { get; set; }
}
