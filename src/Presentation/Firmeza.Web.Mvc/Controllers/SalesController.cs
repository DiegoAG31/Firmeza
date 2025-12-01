using Firmeza.Application.Interfaces;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Enums;
using Firmeza.Infrastructure.Data;
using Firmeza.Web.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.Web.Mvc.Controllers;

[Authorize(Roles = "Admin")]
public class SalesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IPdfService _pdfService;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public SalesController(ApplicationDbContext context, IPdfService pdfService, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _pdfService = pdfService;
        _webHostEnvironment = webHostEnvironment;
    }

    // GET: Sales
    public async Task<IActionResult> Index()
    {
        var sales = await _context.Sales
            .Include(s => s.Customer)
            .OrderByDescending(s => s.SaleDate)
            .ToListAsync();
        return View(sales);
    }

    // GET: Sales/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var sale = await _context.Sales
            .Include(s => s.Customer)
            .Include(s => s.SaleDetails)
            .ThenInclude(sd => sd.Product)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (sale == null) return NotFound();

        return View(sale);
    }

    // GET: Sales/Create
    public IActionResult Create()
    {
        ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "FullName");
        ViewBag.Products = _context.Products.Where(p => p.IsActive && p.Stock > 0).ToList();
        return View();
    }

    // POST: Sales/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSaleViewModel model)
    {
        if (ModelState.IsValid && model.Details.Any())
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Get Customer
                var customer = await _context.Customers.FindAsync(model.CustomerId);
                if (customer == null) throw new Exception("Cliente no encontrado.");

                // 2. Create Sale Header
                var sale = new Sale
                {
                    SaleNumber = $"V-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}",
                    SaleDate = DateTime.UtcNow,
                    CustomerId = model.CustomerId,
                    Customer = customer, // Assign navigation property for PDF
                    Status = SaleStatus.Completed
                };

                decimal subtotal = 0;
                var saleDetails = new List<SaleDetail>();

                // 3. Process Details
                foreach (var item in model.Details)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product == null || product.Stock < item.Quantity)
                    {
                        throw new Exception($"Producto {product?.Name ?? "Desconocido"} sin stock suficiente.");
                    }

                    // Update Stock
                    product.Stock -= item.Quantity;
                    _context.Products.Update(product);

                    // Create Detail
                    var detail = new SaleDetail
                    {
                        ProductId = item.ProductId,
                        Product = product, // Assign navigation property for PDF
                        Quantity = item.Quantity,
                        UnitPrice = product.Price,
                        Subtotal = product.Price * item.Quantity
                    };
                    
                    subtotal += detail.Subtotal;
                    saleDetails.Add(detail);
                }

                sale.SaleDetails = saleDetails;
                sale.Subtotal = subtotal;
                sale.Tax = subtotal * 0.19m; // 19% IVA
                sale.Total = subtotal + sale.Tax;

                _context.Sales.Add(sale);
                await _context.SaveChangesAsync();

                // 4. Generate PDF
                var pdfBytes = _pdfService.GenerateSaleReceipt(sale);
                var pdfFileName = $"Recibo_{sale.SaleNumber}.pdf";
                var pdfPath = Path.Combine(_webHostEnvironment.WebRootPath, "recibos", pdfFileName);
                
                // Ensure directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(pdfPath)!);
                
                await System.IO.File.WriteAllBytesAsync(pdfPath, pdfBytes);

                sale.PdfPath = $"/recibos/{pdfFileName}";
                _context.Sales.Update(sale);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return RedirectToAction(nameof(Details), new { id = sale.Id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError("", $"Error al procesar la venta: {ex.Message}");
            }
        }

        ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "FullName", model.CustomerId);
        ViewBag.Products = _context.Products.Where(p => p.IsActive && p.Stock > 0).ToList();
        return View(model);
    }

    // GET: Sales/DownloadPdf/5
    public async Task<IActionResult> DownloadPdf(int id)
    {
        var sale = await _context.Sales.FindAsync(id);
        if (sale == null || string.IsNullOrEmpty(sale.PdfPath)) return NotFound();

        var webRootPath = _webHostEnvironment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var filePath = Path.Combine(webRootPath, sale.PdfPath.TrimStart('/'));
        if (!System.IO.File.Exists(filePath)) return NotFound();

        var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
        return File(fileBytes, "application/pdf", Path.GetFileName(filePath));
    }
}
