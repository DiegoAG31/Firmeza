using Firmeza.Application.DTOs;
using Firmeza.Application.Interfaces;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Enums;
using Firmeza.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.Web.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Customer")]
public class SalesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IPdfService _pdfService;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public SalesController(
        ApplicationDbContext context,
        IPdfService pdfService,
        IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _pdfService = pdfService;
        _webHostEnvironment = webHostEnvironment;
    }

    /// <summary>
    /// Create a new sale (purchase from customer)
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<SaleResponseDto>> CreateSale([FromBody] CreateSaleDto request)
    {
        if (!request.Items.Any())
            return BadRequest(new { message = "Debe agregar al menos un producto" });

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Get Customer
            var customer = await _context.Customers.FindAsync(request.CustomerId);
            if (customer == null)
                return NotFound(new { message = "Cliente no encontrado" });

            // Create Sale
            var sale = new Sale
            {
                SaleNumber = $"V-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}",
                SaleDate = DateTime.UtcNow,
                CustomerId = request.CustomerId,
                Customer = customer,
                Status = SaleStatus.Completed
            };

            decimal subtotal = 0;
            var saleDetails = new List<SaleDetail>();

            // Process Items
            foreach (var item in request.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null)
                    return NotFound(new { message = $"Producto con ID {item.ProductId} no encontrado" });

                if (product.Stock < item.Quantity)
                    return BadRequest(new { message = $"Stock insuficiente para {product.Name}" });

                // Update Stock
                product.Stock -= item.Quantity;
                _context.Products.Update(product);

                // Create Detail
                var detail = new SaleDetail
                {
                    ProductId = item.ProductId,
                    Product = product,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price,
                    Subtotal = product.Price * item.Quantity
                };

                subtotal += detail.Subtotal;
                saleDetails.Add(detail);
            }

            sale.SaleDetails = saleDetails;
            sale.Subtotal = subtotal;
            sale.Tax = subtotal * 0.19m;
            sale.Total = subtotal + sale.Tax;

            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();

            // Generate PDF
            var pdfBytes = _pdfService.GenerateSaleReceipt(sale);
            var pdfFileName = $"Recibo_{sale.SaleNumber}.pdf";
            var pdfPath = Path.Combine(_webHostEnvironment.WebRootPath, "recibos", pdfFileName);

            Directory.CreateDirectory(Path.GetDirectoryName(pdfPath)!);
            await System.IO.File.WriteAllBytesAsync(pdfPath, pdfBytes);

            sale.PdfPath = $"/recibos/{pdfFileName}";
            _context.Sales.Update(sale);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return Ok(new SaleResponseDto
            {
                Id = sale.Id,
                SaleNumber = sale.SaleNumber,
                SaleDate = sale.SaleDate,
                Subtotal = sale.Subtotal,
                Tax = sale.Tax,
                Total = sale.Total,
                Status = sale.Status.ToString(),
                PdfPath = sale.PdfPath
            });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, new { message = "Error al procesar la venta", error = ex.Message });
        }
    }

    /// <summary>
    /// Get customer's sales history
    /// </summary>
    [HttpGet("my-sales")]
    public async Task<ActionResult<IEnumerable<SaleResponseDto>>> GetMySales()
    {
        var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(userEmail))
            return Unauthorized();

        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == userEmail);
        if (customer == null)
            return NotFound(new { message = "Cliente no encontrado" });

        var sales = await _context.Sales
            .Where(s => s.CustomerId == customer.Id)
            .OrderByDescending(s => s.SaleDate)
            .Select(s => new SaleResponseDto
            {
                Id = s.Id,
                SaleNumber = s.SaleNumber,
                SaleDate = s.SaleDate,
                Subtotal = s.Subtotal,
                Tax = s.Tax,
                Total = s.Total,
                Status = s.Status.ToString(),
                PdfPath = s.PdfPath
            })
            .ToListAsync();

        return Ok(sales);
    }
}
