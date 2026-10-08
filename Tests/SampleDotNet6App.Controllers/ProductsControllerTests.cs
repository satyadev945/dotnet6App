using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using SampleDotNet6App.Controllers;
using SampleDotNet6App.Models;
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

        // ── GetProducts ───────────────────────────────────────────────────────

        [Fact]
        public async Task GetProducts_ReturnsOkWithProducts()
        {
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product A", Price = 10m },
                new Product { Id = 2, Name = "Product B", Price = 20m }
            };
            _productServiceMock.Setup(s => s.GetAllProductsAsync())
                               .ReturnsAsync(products);

            var result = await _controller.GetProducts();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProducts = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
            Assert.Equal(2, ((List<Product>)returnedProducts).Count);
        }

        [Fact]
        public async Task GetProducts_EmptyList_ReturnsOkWithEmptyList()
        {
            _productServiceMock.Setup(s => s.GetAllProductsAsync())
                               .ReturnsAsync(new List<Product>());

            var result = await _controller.GetProducts();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        // ── GetProduct ────────────────────────────────────────────────────────

        [Fact]
        public async Task GetProduct_ExistingId_ReturnsOkWithProduct()
        {
            var product = new Product { Id = 1, Name = "Found Product", Price = 50m };
            _productServiceMock.Setup(s => s.GetProductByIdAsync(1))
                               .ReturnsAsync(product);

            var result = await _controller.GetProduct(1);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsType<Product>(okResult.Value);
            Assert.Equal(1, returned.Id);
        }

        [Fact]
        public async Task GetProduct_NonExistentId_ReturnsNotFound()
        {
            _productServiceMock.Setup(s => s.GetProductByIdAsync(999))
                               .ReturnsAsync((Product?)null);

            var result = await _controller.GetProduct(999);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        // ── GetProductsByCategory ─────────────────────────────────────────────

        [Fact]
        public async Task GetProductsByCategory_ValidCategory_ReturnsOkWithProducts()
        {
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Laptop", Category = "Electronics" }
            };
            _productServiceMock.Setup(s => s.GetProductsByCategoryAsync("Electronics"))
                               .ReturnsAsync(products);

            var result = await _controller.GetProductsByCategory("Electronics");

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetProductsByCategory_EmptyCategory_ReturnsOkWithEmptyList()
        {
            _productServiceMock.Setup(s => s.GetProductsByCategoryAsync("Unknown"))
                               .ReturnsAsync(new List<Product>());

            var result = await _controller.GetProductsByCategory("Unknown");

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        // ── SearchProducts ────────────────────────────────────────────────────

        [Fact]
        public async Task SearchProducts_ValidTerm_ReturnsOkWithResults()
        {
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Laptop Pro" }
            };
            _productServiceMock.Setup(s => s.SearchProductsAsync("laptop"))
                               .ReturnsAsync(products);

            var result = await _controller.SearchProducts("laptop");

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task SearchProducts_EmptyTerm_ReturnsBadRequest()
        {
            var result = await _controller.SearchProducts(string.Empty);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task SearchProducts_WhitespaceTerm_ReturnsBadRequest()
        {
            var result = await _controller.SearchProducts("   ");

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        // ── CreateProduct ─────────────────────────────────────────────────────

        [Fact]
        public async Task CreateProduct_ValidProduct_ReturnsCreatedAtAction()
        {
            var product = new Product { Name = "New Product", Price = 10m };
            var created = new Product { Id = 1, Name = "New Product", Price = 10m };

            _productServiceMock.Setup(s => s.CreateProductAsync(product))
                               .ReturnsAsync(created);

            var result = await _controller.CreateProduct(product);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(_controller.GetProduct), createdResult.ActionName);
            Assert.Equal(1, ((Product)createdResult.Value!).Id);
        }

        [Fact]
        public async Task CreateProduct_InvalidModelState_ReturnsBadRequest()
        {
            _controller.ModelState.AddModelError("Name", "Required");
            var product = new Product();

            var result = await _controller.CreateProduct(product);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        // ── UpdateProduct ─────────────────────────────────────────────────────

        [Fact]
        public async Task UpdateProduct_ExistingProduct_ReturnsOkWithUpdatedProduct()
        {
            var product = new Product { Name = "Updated", Price = 20m };
            var updated = new Product { Id = 1, Name = "Updated", Price = 20m };

            _productServiceMock.Setup(s => s.UpdateProductAsync(1, product))
                               .ReturnsAsync(updated);

            var result = await _controller.UpdateProduct(1, product);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsType<Product>(okResult.Value);
            Assert.Equal("Updated", returned.Name);
        }

        [Fact]
        public async Task UpdateProduct_NonExistentProduct_ReturnsNotFound()
        {
            var product = new Product { Name = "X", Price = 1m };
            _productServiceMock.Setup(s => s.UpdateProductAsync(999, product))
                               .ReturnsAsync((Product?)null);

            var result = await _controller.UpdateProduct(999, product);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task UpdateProduct_InvalidModelState_ReturnsBadRequest()
        {
            _controller.ModelState.AddModelError("Name", "Required");
            var product = new Product();

            var result = await _controller.UpdateProduct(1, product);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        // ── DeleteProduct ─────────────────────────────────────────────────────

        [Fact]
        public async Task DeleteProduct_ExistingProduct_ReturnsNoContent()
        {
            _productServiceMock.Setup(s => s.DeleteProductAsync(1))
                               .ReturnsAsync(true);

            var result = await _controller.DeleteProduct(1);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteProduct_NonExistentProduct_ReturnsNotFound()
        {
            _productServiceMock.Setup(s => s.DeleteProductAsync(999))
                               .ReturnsAsync(false);

            var result = await _controller.DeleteProduct(999);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        // ── UpdateStock ───────────────────────────────────────────────────────

        [Fact]
        public async Task UpdateStock_ExistingProduct_ReturnsNoContent()
        {
            _productServiceMock.Setup(s => s.UpdateStockAsync(1, 50))
                               .ReturnsAsync(true);

            var result = await _controller.UpdateStock(1, 50);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateStock_NonExistentProduct_ReturnsNotFound()
        {
            _productServiceMock.Setup(s => s.UpdateStockAsync(999, 50))
                               .ReturnsAsync(false);

            var result = await _controller.UpdateStock(999, 50);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task UpdateStock_ZeroQuantity_ReturnsNoContent()
        {
            _productServiceMock.Setup(s => s.UpdateStockAsync(1, 0))
                               .ReturnsAsync(true);

            var result = await _controller.UpdateStock(1, 0);

            Assert.IsType<NoContentResult>(result);
        }
    }
}
