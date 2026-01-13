using System;
using Xunit;
using SampleDotNet6App.Models;

namespace SampleDotNet6App.Models.Tests;

public class ProductTests
{
    [Fact]
    public void Constructor_InitializesWithDefaultValues()
    {
        // Arrange & Act
        var product = new Product();

        // Assert
        Assert.Equal(0, product.Id);
        Assert.Equal(string.Empty, product.Name);
        Assert.Null(product.Description);
        Assert.Equal(0, product.Price);
        Assert.Equal(0, product.Stock);
        Assert.NotEqual(default(DateTime), product.CreatedDate);
        Assert.Null(product.ModifiedDate);
    }

    [Fact]
    public void Id_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var product = new Product();
        var expectedId = 123;

        // Act
        product.Id = expectedId;

        // Assert
        Assert.Equal(expectedId, product.Id);
    }

    [Fact]
    public void Name_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var product = new Product();
        var expectedName = "Test Product";

        // Act
        product.Name = expectedName;

        // Assert
        Assert.Equal(expectedName, product.Name);
    }

    [Fact]
    public void Name_SetEmpty_ReturnsEmpty()
    {
        // Arrange
        var product = new Product();

        // Act
        product.Name = string.Empty;

        // Assert
        Assert.Equal(string.Empty, product.Name);
    }

    [Fact]
    public void Description_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var product = new Product();
        var expectedDescription = "Test Description";

        // Act
        product.Description = expectedDescription;

        // Assert
        Assert.Equal(expectedDescription, product.Description);
    }

    [Fact]
    public void Description_SetNull_ReturnsNull()
    {
        // Arrange
        var product = new Product();

        // Act
        product.Description = null;

        // Assert
        Assert.Null(product.Description);
    }

    [Fact]
    public void Price_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var product = new Product();
        var expectedPrice = 99.99m;

        // Act
        product.Price = expectedPrice;

        // Assert
        Assert.Equal(expectedPrice, product.Price);
    }

    [Fact]
    public void Price_SetZero_ReturnsZero()
    {
        // Arrange
        var product = new Product();

        // Act
        product.Price = 0;

        // Assert
        Assert.Equal(0, product.Price);
    }

    [Fact]
    public void Price_SetNegative_ReturnsNegative()
    {
        // Arrange
        var product = new Product();
        var negativePrice = -10.5m;

        // Act
        product.Price = negativePrice;

        // Assert
        Assert.Equal(negativePrice, product.Price);
    }

    [Fact]
    public void Stock_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var product = new Product();
        var expectedStock = 100;

        // Act
        product.Stock = expectedStock;

        // Assert
        Assert.Equal(expectedStock, product.Stock);
    }

    [Fact]
    public void Stock_SetZero_ReturnsZero()
    {
        // Arrange
        var product = new Product();

        // Act
        product.Stock = 0;

        // Assert
        Assert.Equal(0, product.Stock);
    }

    [Fact]
    public void Stock_SetNegative_ReturnsNegative()
    {
        // Arrange
        var product = new Product();
        var negativeStock = -5;

        // Act
        product.Stock = negativeStock;

        // Assert
        Assert.Equal(negativeStock, product.Stock);
    }

    [Fact]
    public void CreatedDate_InitializedToUtcNow()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow.AddSeconds(-1);

        // Act
        var product = new Product();
        var afterCreation = DateTime.UtcNow.AddSeconds(1);

        // Assert
        Assert.True(product.CreatedDate >= beforeCreation);
        Assert.True(product.CreatedDate <= afterCreation);
    }

    [Fact]
    public void CreatedDate_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var product = new Product();
        var expectedDate = new DateTime(2024, 1, 1);

        // Act
        product.CreatedDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, product.CreatedDate);
    }

    [Fact]
    public void ModifiedDate_SetAndGet_ReturnsCorrectValue()
    {
        // Arrange
        var product = new Product();
        var expectedDate = new DateTime(2024, 6, 15);

        // Act
        product.ModifiedDate = expectedDate;

        // Assert
        Assert.Equal(expectedDate, product.ModifiedDate);
    }

    [Fact]
    public void ModifiedDate_SetNull_ReturnsNull()
    {
        // Arrange
        var product = new Product();

        // Act
        product.ModifiedDate = null;

        // Assert
        Assert.Null(product.ModifiedDate);
    }

    [Fact]
    public void AllProperties_SetAndGet_ReturnsCorrectValues()
    {
        // Arrange
        var product = new Product();
        var id = 1;
        var name = "Laptop";
        var description = "High performance";
        var price = 1299.99m;
        var stock = 50;
        var createdDate = DateTime.UtcNow;
        var modifiedDate = DateTime.UtcNow.AddDays(1);

        // Act
        product.Id = id;
        product.Name = name;
        product.Description = description;
        product.Price = price;
        product.Stock = stock;
        product.CreatedDate = createdDate;
        product.ModifiedDate = modifiedDate;

        // Assert
        Assert.Equal(id, product.Id);
        Assert.Equal(name, product.Name);
        Assert.Equal(description, product.Description);
        Assert.Equal(price, product.Price);
        Assert.Equal(stock, product.Stock);
        Assert.Equal(createdDate, product.CreatedDate);
        Assert.Equal(modifiedDate, product.ModifiedDate);
    }
}
