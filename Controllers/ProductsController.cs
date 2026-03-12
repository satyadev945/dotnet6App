using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SampleDotNet6App.DTOs;
using SampleDotNet6App.Services;

namespace SampleDotNet6App.Controllers
{
    /// <summary>
    /// Controller for product operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductService productService, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        /// <summary>
        /// Get all products
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
        {
            try
            {
                _logger.LogInformation("Getting all products");
                var products = await _productService.GetAllProductsAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all products");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "An error occurred while retrieving products" });
            }
        }

        /// <summary>
        /// Get product by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDto>> GetById(int id)
        {
            try
            {
                _logger.LogInformation("Getting product with ID: {ProductId}", id);
                var product = await _productService.GetProductByIdAsync(id);
                
                if (product == null)
                {
                    return NotFound(new { message = $"Product with ID {id} not found" });
                }

                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product with ID: {ProductId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "An error occurred while retrieving the product" });
            }
        }

        /// <summary>
        /// Search products
        /// </summary>
        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductDto>>> Search([FromQuery] string q)
        {
            try
            {
                _logger.LogInformation("Searching products with term: {SearchTerm}", q);
                var products = await _productService.SearchProductsAsync(q);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching products");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "An error occurred while searching products" });
            }
        }

        /// <summary>
        /// Create a new product (Admin/Manager only)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto createDto)
        {
            try
            {
                _logger.LogInformation("Creating new product: {ProductName}", createDto.Name);
                var product = await _productService.CreateProductAsync(createDto);
                return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "An error occurred while creating the product" });
            }
        }

        /// <summary>
        /// Update a product (Admin/Manager only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDto>> Update(int id, [FromBody] UpdateProductDto updateDto)
        {
            try
            {
                _logger.LogInformation("Updating product with ID: {ProductId}", id);
                var product = await _productService.UpdateProductAsync(id, updateDto);
                
                if (product == null)
                {
                    return NotFound(new { message = $"Product with ID {id} not found" });
                }

                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product with ID: {ProductId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "An error occurred while updating the product" });
            }
        }

        /// <summary>
        /// Delete a product (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                _logger.LogInformation("Deleting product with ID: {ProductId}", id);
                var result = await _productService.DeleteProductAsync(id);
                
                if (!result)
                {
                    return NotFound(new { message = $"Product with ID {id} not found" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product with ID: {ProductId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "An error occurred while deleting the product" });
            }
        }
    }
}
