using SampleDotNet6App.Models;
using Xunit;

namespace SampleDotNet6App.Models.Tests;

public class ProductTests
{
    [Fact]
    public void Product_Constructor_InitializesWithDefaultValues()
    {
        // Arrange & Act
        var product = new Product();

        // Assert
        Assert.Equal(0, product.Id);
        Assert.Equal(string.Empty, product.Name);
        Assert.Null(product.Description);
        Assert.Equal(0m, product.Price);
        Assert.Equal(0, product.Stock);
    }

    [Fact]
    public void Product_SetId_UpdatesIdProperty()
    {
        // Arrange
        var product = new Product();

        // Act
        product.Id = 123;

        // Assert
        Assert.Equal(123, product.Id);
    }

    [Fact]
    public void Product_SetName_UpdatesNameProperty()
    {
        // Arrange
        var product = new Product();

        // Act
        product.Name = "Test Product";

        // Assert
        Assert.Equal("Test Product", product.Name);
    }

    [Fact]
    public void Product_SetDescription_UpdatesDescriptionProperty()
    {
        // Arrange
        var product = new Product();

        // Act
        product.Description = "Test Description";

        // Assert
        Assert.Equal("Test Description", product.Description);
    }

    [Fact]
    public void Product_SetDescriptionNull_AllowsNullValue()
    {
        // Arrange
        var product = new Product { Description = "Initial" };

        // Act
        product.Description = null;

        // Assert
        Assert.Null(product.Description);
    }

    [Fact]
    public void Product_SetPrice_UpdatesPriceProperty()
    {
        // Arrange
        var product = new Product();

        // Act
        product.Price = 99.99m;

        // Assert
        Assert.Equal(99.99m, product.Price);
    }

    [Fact]
    public void Product_SetPriceZero_AcceptsZeroValue()
    {
        // Arrange
        var product = new Product();

        // Act
        product.Price = 0m;

        // Assert
        Assert.Equal(0m, product.Price);
    }

    [Fact]
    public void Product_SetPriceNegative_AcceptsNegativeValue()
    {
        // Arrange
        var product = new Product();

        // Act
        product.Price = -10m;

        // Assert
        Assert.Equal(-10m, product.Price);
    }

    [Fact]
    public void Product_SetStock_UpdatesStockProperty()
    {
        // Arrange
        var product = new Product();

        // Act
        product.Stock = 100;

        // Assert
        Assert.Equal(100, product.Stock);
    }

    [Fact]
    public void Product_SetStockNegative_AcceptsNegativeValue()
    {
        // Arrange
        var product = new Product();

        // Act
        product.Stock = -5;

        // Assert
        Assert.Equal(-5, product.Stock);
    }

    [Fact]
    public void Product_SetAllProperties_RetainsValues()
    {
        // Arrange
        var product = new Product();

        // Act
        product.Id = 42;
        product.Name = "Complete Product";
        product.Description = "Full Description";
        product.Price = 49.99m;
        product.Stock = 250;

        // Assert
        Assert.Equal(42, product.Id);
        Assert.Equal("Complete Product", product.Name);
        Assert.Equal("Full Description", product.Description);
        Assert.Equal(49.99m, product.Price);
        Assert.Equal(250, product.Stock);
    }

    [Fact]
    public void Product_InitializerSyntax_SetsPropertiesCorrectly()
    {
        // Arrange & Act
        var product = new Product
        {
            Id = 1,
            Name = "Initialized Product",
            Description = "Init Description",
            Price = 29.99m,
            Stock = 50
        };

        // Assert
        Assert.Equal(1, product.Id);
        Assert.Equal("Initialized Product", product.Name);
        Assert.Equal("Init Description", product.Description);
        Assert.Equal(29.99m, product.Price);
        Assert.Equal(50, product.Stock);
    }
}
