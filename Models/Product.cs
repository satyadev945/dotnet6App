namespace SampleDotNet6App.Models;

/// <summary>
/// Represents a product in the system
/// </summary>
public class Product
{
    /// <summary>
    /// Gets or sets the product identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the product name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the product description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the product price
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Gets or sets the quantity in stock
    /// </summary>
    public int QuantityInStock { get; set; }

    /// <summary>
    /// Gets or sets the product category
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Gets or sets the date when the product was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets whether the product is active
    /// </summary>
    public bool IsActive { get; set; } = true;
}
