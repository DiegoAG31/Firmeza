using Firmeza.Infrastructure.Data;
using Firmeza.Web.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Firmeza.Web.Mvc.Controllers;

[Authorize(Roles = "Admin")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var dashboardMetrics = new
        {
            TotalProducts = await _context.Products.CountAsync(),
            TotalCustomers = await _context.Customers.CountAsync(),
            TotalSales = await _context.Sales.CountAsync(),
            TotalVehicles = await _context.Vehicles.CountAsync(),
            LowStockProducts = await _context.Products.CountAsync(p => p.Stock < 10)
        };

        ViewBag.Metrics = dashboardMetrics;
        return View();
    }

    public async Task<IActionResult> GenerateReport()
    {
        // Configure EPPlus License
        OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

        var products = await _context.Products
            .Include(p => p.Category)
            .OrderBy(p => p.Name)
            .ToListAsync();

        using var package = new OfficeOpenXml.ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Inventario");

        // Headers
        worksheet.Cells[1, 1].Value = "Código";
        worksheet.Cells[1, 2].Value = "Producto";
        worksheet.Cells[1, 3].Value = "Categoría";
        worksheet.Cells[1, 4].Value = "Precio";
        worksheet.Cells[1, 5].Value = "Stock";
        worksheet.Cells[1, 6].Value = "Estado";

        // Style Header
        using (var range = worksheet.Cells[1, 1, 1, 6])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        }

        // Data
        int row = 2;
        foreach (var item in products)
        {
            worksheet.Cells[row, 1].Value = item.Code;
            worksheet.Cells[row, 2].Value = item.Name;
            worksheet.Cells[row, 3].Value = item.Category?.Name;
            worksheet.Cells[row, 4].Value = item.Price;
            worksheet.Cells[row, 5].Value = item.Stock;
            worksheet.Cells[row, 6].Value = item.IsActive ? "Activo" : "Inactivo";
            row++;
        }

        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        await package.SaveAsAsync(stream);
        stream.Position = 0;

        var fileName = $"Inventario_Firmeza_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
