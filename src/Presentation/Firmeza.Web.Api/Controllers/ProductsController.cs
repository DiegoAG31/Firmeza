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

    public ProductsController(ApplicationDbContext context)
    {
        _context = context;
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
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Code = p.Code,
                Price = p.Price,
                Stock = p.Stock,
                Type = p.Type,
                ImageUrl = p.ImageUrl,
                CategoryId = p.CategoryId ?? 0,
                CategoryName = p.Category!.Name,
                IsActive = p.IsActive
            })
            .ToListAsync();

        return Ok(products);
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
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Code = p.Code,
                Price = p.Price,
                Stock = p.Stock,
                Type = p.Type,
                ImageUrl = p.ImageUrl,
                CategoryId = p.CategoryId ?? 0,
                CategoryName = p.Category!.Name,
                IsActive = p.IsActive
            })
            .FirstOrDefaultAsync();

        if (product == null)
            return NotFound(new { message = "Producto no encontrado" });

        return Ok(product);
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
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Code = p.Code,
                Price = p.Price,
                Stock = p.Stock,
                Type = p.Type,
                ImageUrl = p.ImageUrl,
                CategoryId = p.CategoryId ?? 0,
                CategoryName = p.Category!.Name,
                IsActive = p.IsActive
            })
            .ToListAsync();

        return Ok(products);
    }
}
