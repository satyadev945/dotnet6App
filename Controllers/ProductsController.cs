using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SampleDotNet6App.Mappings;
using SampleDotNet6App.Models;
using SampleDotNet6App.Services;

namespace SampleDotNet6App.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductService productService, IMapper mapper, ILogger<ProductsController> logger)
    {
        _productService = productService;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Get all active products
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
    {
        var products = await _productService.GetAllAsync();
        return Ok(_mapper.Map<IEnumerable<ProductDto>>(products));
    }

    /// <summary>
    /// Get product by ID
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null)
            return NotFound(new { message = $"Product with ID {id} not found." });

        return Ok(_mapper.Map<ProductDto>(product));
    }

    /// <summary>
    /// Search products by name, description, or category
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> Search([FromQuery] string q = "")
    {
        var products = await _productService.SearchAsync(q);
        return Ok(_mapper.Map<IEnumerable<ProductDto>>(products));
    }

    /// <summary>
    /// Create a new product (Admin/Manager only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto dto)
    {
        var product = _mapper.Map<Product>(dto);
        var created = await _productService.CreateAsync(product);
        var result = _mapper.Map<ProductDto>(created);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Update an existing product (Admin/Manager only)
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ProductDto>> Update(int id, [FromBody] CreateProductDto dto)
    {
        var product = _mapper.Map<Product>(dto);
        var updated = await _productService.UpdateAsync(id, product);
        if (updated == null)
            return NotFound(new { message = $"Product with ID {id} not found." });

        return Ok(_mapper.Map<ProductDto>(updated));
    }

    /// <summary>
    /// Delete a product (Admin only)
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _productService.DeleteAsync(id);
        if (!deleted)
            return NotFound(new { message = $"Product with ID {id} not found." });

        return NoContent();
    }
}
