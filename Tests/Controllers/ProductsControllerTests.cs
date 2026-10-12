using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using SampleDotNet6App.Controllers;
using SampleDotNet6App.Mappings;
using SampleDotNet6App.Models;
using SampleDotNet6App.Services;

namespace SampleDotNet6App.Tests.Controllers
{
    public class ProductsControllerTests
    {
        private readonly Mock<IProductService> _mockProductService;
        private readonly IMapper _mapper;
        private readonly Mock<ILogger<ProductsController>> _mockLogger;
        private readonly ProductsController _controller;

        public ProductsControllerTests()
        {
            _mockProductService = new Mock<IProductService>();
            _mockLogger = new Mock<ILogger<ProductsController>>();

            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();

            _controller = new ProductsController(
                _mockProductService.Object,
                _mapper,
                _mockLogger.Object);
        }

        private static Product CreateTestProduct(int id = 1, string name = "Test Product")
        {
            return new Product
            {
                Id = id,
                Name = name,
                Description = "A test product",
                Price = 99.99m,
                Category = "Electronics",
                StockQuantity = 10,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
        }

        // ─── GetAll ────────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetAll_ReturnsOkWithProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                CreateTestProduct(1, "Product A"),
                CreateTestProduct(2, "Product B")
            };
            _mockProductService.Setup(s => s.GetAllAsync()).ReturnsAsync(products);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetAll_EmptyList_ReturnsOkWithEmptyList()
        {
            // Arrange
            _mockProductService.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<Product>());

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        // ─── GetById ───────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetById_ExistingProduct_ReturnsOk()
        {
            // Arrange
            var product = CreateTestProduct(id: 1);
            _mockProductService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(product);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetById_NonExistentProduct_ReturnsNotFound()
        {
            // Arrange
            _mockProductService.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((Product?)null);

            // Act
            var result = await _controller.GetById(999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        // ─── Search ────────────────────────────────────────────────────────────────

        [Fact]
        public async Task Search_WithSearchTerm_ReturnsOkWithMatchingProducts()
        {
            // Arrange
            var products = new List<Product> { CreateTestProduct(1, "Laptop") };
            _mockProductService.Setup(s => s.SearchAsync("laptop")).ReturnsAsync(products);

            // Act
            var result = await _controller.Search("laptop");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task Search_EmptySearchTerm_ReturnsOkWithAllProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                CreateTestProduct(1, "Product A"),
                CreateTestProduct(2, "Product B")
            };
            _mockProductService.Setup(s => s.SearchAsync("")).ReturnsAsync(products);

            // Act
            var result = await _controller.Search("");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task Search_NoMatches_ReturnsOkWithEmptyList()
        {
            // Arrange
            _mockProductService.Setup(s => s.SearchAsync("xyz")).ReturnsAsync(new List<Product>());

            // Act
            var result = await _controller.Search("xyz");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        // ─── Create ────────────────────────────────────────────────────────────────

        [Fact]
        public async Task Create_ValidProduct_ReturnsCreatedAtAction()
        {
            // Arrange
            var dto = new CreateProductDto
            {
                Name = "New Product",
                Description = "A new product",
                Price = 49.99m,
                Category = "Test",
                StockQuantity = 5
            };
            var createdProduct = CreateTestProduct(id: 10, name: "New Product");
            _mockProductService.Setup(s => s.CreateAsync(It.IsAny<Product>())).ReturnsAsync(createdProduct);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(_controller.GetById), createdResult.ActionName);
        }

        [Fact]
        public async Task Create_ValidProduct_ReturnsProductDto()
        {
            // Arrange
            var dto = new CreateProductDto { Name = "Product", Description = "desc", Price = 10m, Category = "Cat", StockQuantity = 1 };
            var createdProduct = CreateTestProduct(id: 1, name: "Product");
            _mockProductService.Setup(s => s.CreateAsync(It.IsAny<Product>())).ReturnsAsync(createdProduct);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var productDto = Assert.IsType<ProductDto>(createdResult.Value);
            Assert.Equal("Product", productDto.Name);
        }

        // ─── Update ────────────────────────────────────────────────────────────────

        [Fact]
        public async Task Update_ExistingProduct_ReturnsOk()
        {
            // Arrange
            var dto = new CreateProductDto { Name = "Updated", Description = "desc", Price = 10m, Category = "Cat", StockQuantity = 1 };
            var updatedProduct = CreateTestProduct(id: 1, name: "Updated");
            _mockProductService.Setup(s => s.UpdateAsync(1, It.IsAny<Product>())).ReturnsAsync(updatedProduct);

            // Act
            var result = await _controller.Update(1, dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task Update_NonExistentProduct_ReturnsNotFound()
        {
            // Arrange
            var dto = new CreateProductDto { Name = "Ghost", Description = "desc", Price = 10m, Category = "Cat", StockQuantity = 1 };
            _mockProductService.Setup(s => s.UpdateAsync(999, It.IsAny<Product>())).ReturnsAsync((Product?)null);

            // Act
            var result = await _controller.Update(999, dto);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        // ─── Delete ────────────────────────────────────────────────────────────────

        [Fact]
        public async Task Delete_ExistingProduct_ReturnsNoContent()
        {
            // Arrange
            _mockProductService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_NonExistentProduct_ReturnsNotFound()
        {
            // Arrange
            _mockProductService.Setup(s => s.DeleteAsync(999)).ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
