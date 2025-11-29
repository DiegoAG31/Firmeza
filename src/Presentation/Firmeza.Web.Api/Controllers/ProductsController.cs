using AutoMapper;
using Firmeza.Application.DTOs;
using Firmeza.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.Web.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public ProductsController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Get all active products
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
    {
        var products = await _context.Products
            .Include(p => p.Category)
            .Where(p => p.IsActive && p.Stock > 0)
            .ToListAsync();

        var productsDto = _mapper.Map<IEnumerable<ProductDto>>(products);
        return Ok(productsDto);
    }

    /// <summary>
    /// Get product by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .Where(p => p.Id == id && p.IsActive)
            .FirstOrDefaultAsync();

        if (product == null)
            return NotFound(new { message = "Producto no encontrado" });

        var productDto = _mapper.Map<ProductDto>(product);
        return Ok(productDto);
    }

    /// <summary>
    /// Get products by category
    /// </summary>
    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByCategory(int categoryId)
    {
        var products = await _context.Products
            .Include(p => p.Category)
            .Where(p => p.CategoryId == categoryId && p.IsActive && p.Stock > 0)
            .ToListAsync();

        var productsDto = _mapper.Map<IEnumerable<ProductDto>>(products);
        return Ok(productsDto);
    }
}
