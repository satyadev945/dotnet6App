using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using SampleDotNet6App.Controllers;
using SampleDotNet6App.DTOs;
using SampleDotNet6App.Services;

namespace SampleDotNet6App.Controllers.Tests
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

        // ─── GetAll ───────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetAll_ReturnsOkWithProducts()
        {
            // Arrange
            var products = new List<ProductDto>
            {
                new ProductDto { Id = 1, Name = "Product A" },
                new ProductDto { Id = 2, Name = "Product B" }
            };
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
        public async Task GetAll_EmptyList_ReturnsOkWithEmptyList()
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

        // ─── GetById ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetById_ExistingId_ReturnsOkWithProduct()
        {
            // Arrange
            var product = new ProductDto { Id = 1, Name = "Widget" };
            _productServiceMock.Setup(s => s.GetProductByIdAsync(1))
                .ReturnsAsync(product);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProduct = Assert.IsType<ProductDto>(okResult.Value);
            Assert.Equal("Widget", returnedProduct.Name);
        }

        [Fact]
        public async Task GetById_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            _productServiceMock.Setup(s => s.GetProductByIdAsync(999))
                .ReturnsAsync((ProductDto?)null);

            // Act
            var result = await _controller.GetById(999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        // ─── Search ───────────────────────────────────────────────────────────────

        [Fact]
        public async Task Search_ValidTerm_ReturnsOkWithResults()
        {
            // Arrange
            var products = new List<ProductDto> { new ProductDto { Id = 1, Name = "Laptop" } };
            _productServiceMock.Setup(s => s.SearchProductsAsync("Laptop"))
                .ReturnsAsync(products);

            // Act
            var result = await _controller.Search("Laptop");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task Search_EmptyTerm_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.Search("");

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task Search_WhitespaceTerm_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.Search("   ");

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task Search_NullTerm_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.Search(null!);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        // ─── Create ───────────────────────────────────────────────────────────────

        [Fact]
        public async Task Create_ValidDto_ReturnsCreatedAtAction()
        {
            // Arrange
            var createDto = new CreateProductDto { Name = "New Product", Price = 9.99m };
            var createdProduct = new ProductDto { Id = 1, Name = "New Product", Price = 9.99m };
            _productServiceMock.Setup(s => s.CreateProductAsync(createDto))
                .ReturnsAsync(createdProduct);

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(_controller.GetById), createdResult.ActionName);
            var returnedProduct = Assert.IsType<ProductDto>(createdResult.Value);
            Assert.Equal("New Product", returnedProduct.Name);
        }

        // ─── Update ───────────────────────────────────────────────────────────────

        [Fact]
        public async Task Update_ExistingId_ReturnsOkWithUpdatedProduct()
        {
            // Arrange
            var updateDto = new UpdateProductDto { Name = "Updated" };
            var updatedProduct = new ProductDto { Id = 1, Name = "Updated" };
            _productServiceMock.Setup(s => s.UpdateProductAsync(1, updateDto))
                .ReturnsAsync(updatedProduct);

            // Act
            var result = await _controller.Update(1, updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProduct = Assert.IsType<ProductDto>(okResult.Value);
            Assert.Equal("Updated", returnedProduct.Name);
        }

        [Fact]
        public async Task Update_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var updateDto = new UpdateProductDto { Name = "X" };
            _productServiceMock.Setup(s => s.UpdateProductAsync(999, updateDto))
                .ReturnsAsync((ProductDto?)null);

            // Act
            var result = await _controller.Update(999, updateDto);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        // ─── Delete ───────────────────────────────────────────────────────────────

        [Fact]
        public async Task Delete_ExistingId_ReturnsNoContent()
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
        public async Task Delete_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            _productServiceMock.Setup(s => s.DeleteProductAsync(999))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
