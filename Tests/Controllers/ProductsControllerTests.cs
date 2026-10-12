using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using SampleDotNet6App.Controllers;
using SampleDotNet6App.Models;
using SampleDotNet6App.Services;

namespace SampleDotNet6App.Tests.Controllers
{
    public class ProductsControllerTests
    {
        private readonly Mock<IProductService> _productServiceMock;
        private readonly Mock<ILogger<ProductsController>> _loggerMock;
        private readonly ProductsController _controller;

        public ProductsControllerTests()
        {
            _productServiceMock = new Mock<IProductService>();
            _loggerMock = new Mock<ILogger<ProductsController>>();
            _controller = new ProductsController(_productServiceMock.Object, _loggerMock.Object);
        }

        private static ProductDto CreateProductDto(int id = 1, string name = "Test Product", bool isActive = true)
        {
            return new ProductDto
            {
                Id = id,
                Name = name,
                Description = "Test Description",
                Price = 99.99m,
                Category = "Electronics",
                StockQuantity = 10,
                IsActive = isActive,
                CreatedAt = DateTime.UtcNow
            };
        }

        // ---- GetAll Tests ----
        [Fact]
        public async Task GetAll_ReturnsOkWithProducts()
        {
            // Arrange
            var products = new List<ProductDto> { CreateProductDto(1), CreateProductDto(2) };
            _productServiceMock.Setup(s => s.GetAllProductsAsync())
                .ReturnsAsync(products);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProducts = Assert.IsAssignableFrom<IEnumerable<ProductDto>>(okResult.Value);
            Assert.Equal(2, ((List<ProductDto>)returnedProducts).Count);
        }

        [Fact]
        public async Task GetAll_WithEmptyList_ReturnsOkWithEmptyList()
        {
            // Arrange
            _productServiceMock.Setup(s => s.GetAllProductsAsync())
                .ReturnsAsync(new List<ProductDto>());

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        // ---- GetById Tests ----
        [Fact]
        public async Task GetById_WithValidId_ReturnsOkWithProduct()
        {
            // Arrange
            var product = CreateProductDto(1);
            _productServiceMock.Setup(s => s.GetProductByIdAsync(1))
                .ReturnsAsync(product);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProduct = Assert.IsType<ProductDto>(okResult.Value);
            Assert.Equal(1, returnedProduct.Id);
        }

        [Fact]
        public async Task GetById_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            _productServiceMock.Setup(s => s.GetProductByIdAsync(9999))
                .ReturnsAsync((ProductDto?)null);

            // Act
            var result = await _controller.GetById(9999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        // ---- Search Tests ----
        [Fact]
        public async Task Search_WithValidQuery_ReturnsOkWithProducts()
        {
            // Arrange
            var products = new List<ProductDto> { CreateProductDto(1, "Laptop") };
            _productServiceMock.Setup(s => s.SearchProductsAsync("Laptop"))
                .ReturnsAsync(products);

            // Act
            var result = await _controller.Search("Laptop");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task Search_WithNoResults_ReturnsOkWithEmptyList()
        {
            // Arrange
            _productServiceMock.Setup(s => s.SearchProductsAsync("XYZ"))
                .ReturnsAsync(new List<ProductDto>());

            // Act
            var result = await _controller.Search("XYZ");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        // ---- GetByCategory Tests ----
        [Fact]
        public async Task GetByCategory_WithValidCategory_ReturnsOkWithProducts()
        {
            // Arrange
            var products = new List<ProductDto> { CreateProductDto(1), CreateProductDto(2) };
            _productServiceMock.Setup(s => s.GetProductsByCategoryAsync("Electronics"))
                .ReturnsAsync(products);

            // Act
            var result = await _controller.GetByCategory("Electronics");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        // ---- Create Tests ----
        [Fact]
        public async Task Create_WithValidData_ReturnsCreatedAtAction()
        {
            // Arrange
            var createDto = new CreateProductDto
            {
                Name = "New Product",
                Price = 49.99m,
                Category = "Gadgets",
                StockQuantity = 20
            };
            var createdProduct = CreateProductDto(5, "New Product");
            _productServiceMock.Setup(s => s.CreateProductAsync(createDto))
                .ReturnsAsync(createdProduct);

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(_controller.GetById), createdResult.ActionName);
            Assert.Equal(5, ((ProductDto)createdResult.Value!).Id);
        }

        [Fact]
        public async Task Create_WithInvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("Name", "Name is required");
            var createDto = new CreateProductDto();

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        // ---- Update Tests ----
        [Fact]
        public async Task Update_WithValidId_ReturnsOkWithUpdatedProduct()
        {
            // Arrange
            var updateDto = new UpdateProductDto { Name = "Updated Name" };
            var updatedProduct = CreateProductDto(1, "Updated Name");
            _productServiceMock.Setup(s => s.UpdateProductAsync(1, updateDto))
                .ReturnsAsync(updatedProduct);

            // Act
            var result = await _controller.Update(1, updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProduct = Assert.IsType<ProductDto>(okResult.Value);
            Assert.Equal("Updated Name", returnedProduct.Name);
        }

        [Fact]
        public async Task Update_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var updateDto = new UpdateProductDto { Name = "Test" };
            _productServiceMock.Setup(s => s.UpdateProductAsync(9999, updateDto))
                .ReturnsAsync((ProductDto?)null);

            // Act
            var result = await _controller.Update(9999, updateDto);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        // ---- Delete Tests ----
        [Fact]
        public async Task Delete_WithValidId_ReturnsNoContent()
        {
            // Arrange
            _productServiceMock.Setup(s => s.DeleteProductAsync(1))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            _productServiceMock.Setup(s => s.DeleteProductAsync(9999))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(9999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
