using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SampleDotNet6App.Models;
using SampleDotNet6App.Services;

namespace SampleDotNet6App.Controllers
{
    /// <summary>
    /// Products API controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="productService">Product service</param>
        /// <param name="logger">Logger</param>
        public ProductsController(IProductService productService, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        /// <summary>
        /// Get all products
        /// </summary>
        /// <returns>List of products</returns>
        /// <response code="200">Returns the list of products</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            _logger.LogInformation("Getting all products");
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        /// <summary>
        /// Get product by ID
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Product details</returns>
        /// <response code="200">Returns the product</response>
        /// <response code="404">Product not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            _logger.LogInformation("Getting product with ID: {ProductId}", id);
            var product = await _productService.GetProductByIdAsync(id);

            if (product == null)
            {
                _logger.LogWarning("Product with ID {ProductId} not found", id);
                return NotFound();
            }

            return Ok(product);
        }

        /// <summary>
        /// Search products by name
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching products</returns>
        /// <response code="200">Returns the list of matching products</response>
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Product>>> SearchProducts([FromQuery] string searchTerm)
        {
            _logger.LogInformation("Searching products with term: {SearchTerm}", searchTerm);
            var products = await _productService.SearchProductsAsync(searchTerm);
            return Ok(products);
        }

        /// <summary>
        /// Create a new product
        /// </summary>
        /// <param name="product">Product to create</param>
        /// <returns>Created product</returns>
        /// <response code="201">Product created successfully</response>
        /// <response code="400">Invalid product data</response>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Creating new product: {ProductName}", product.Name);
            var createdProduct = await _productService.CreateProductAsync(product);

            return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, createdProduct);
        }

        /// <summary>
        /// Update an existing product
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="product">Updated product data</param>
        /// <returns>Updated product</returns>
        /// <response code="200">Product updated successfully</response>
        /// <response code="400">Invalid product data</response>
        /// <response code="404">Product not found</response>
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Product>> UpdateProduct(int id, [FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Updating product with ID: {ProductId}", id);
            var updatedProduct = await _productService.UpdateProductAsync(id, product);

            if (updatedProduct == null)
            {
                _logger.LogWarning("Product with ID {ProductId} not found for update", id);
                return NotFound();
            }

            return Ok(updatedProduct);
        }

        /// <summary>
        /// Delete a product
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>No content</returns>
        /// <response code="204">Product deleted successfully</response>
        /// <response code="404">Product not found</response>
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            _logger.LogInformation("Deleting product with ID: {ProductId}", id);
            var result = await _productService.DeleteProductAsync(id);

            if (!result)
            {
                _logger.LogWarning("Product with ID {ProductId} not found for deletion", id);
                return NotFound();
            }

            return NoContent();
        }
    }
}
