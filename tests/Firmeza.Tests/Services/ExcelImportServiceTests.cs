using Firmeza.Application.DTOs;
using Firmeza.Domain.Entities;
using Firmeza.Infrastructure.Services;
using Firmeza.Tests.Helpers;
using OfficeOpenXml;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.Tests.Services;

public class ExcelImportServiceTests : IDisposable
{
    private readonly Infrastructure.Data.ApplicationDbContext _context;
    private readonly ExcelImportService _service;

    public ExcelImportServiceTests()
    {
        _context = TestDbContextFactory.CreateInMemoryContext();
        _service = new ExcelImportService(_context);
        
        // Configure EPPlus license
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    [Fact]
    public async Task ImportDataAsync_WithValidData_ShouldImportSuccessfully()
    {
        // Arrange
        var excelStream = CreateTestExcelFile(new[]
        {
            new { Codigo = "PROD001", Producto = "Cemento", Precio = "25000", Stock = "100", 
                  Documento = "123456", Cliente = "Juan Perez", Email = "juan@email.com", Cantidad = "5" }
        });

        // Act
        var result = await _service.ImportDataAsync(excelStream);

        // Assert
        Assert.Equal(1, result.SuccessCount);
        Assert.Equal(0, result.ErrorCount);
        Assert.Empty(result.Errors);
        
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Code == "PROD001");
        Assert.NotNull(product);
        Assert.Equal("Cemento", product.Name);
        Assert.Equal(25000, product.Price);
    }

    [Fact]
    public async Task ImportDataAsync_WithDuplicateCustomers_ShouldReuseSameCustomer()
    {
        // Arrange
        var excelStream = CreateTestExcelFile(new[]
        {
            new { Codigo = "PROD001", Producto = "Cemento", Precio = "25000", Stock = "100", 
                  Documento = "123456", Cliente = "Juan Perez", Email = "juan@email.com", Cantidad = "5" },
            new { Codigo = "PROD002", Producto = "Arena", Precio = "45000", Stock = "50", 
                  Documento = "123456", Cliente = "Juan Perez", Email = "juan@email.com", Cantidad = "3" }
        });

        // Act
        var result = await _service.ImportDataAsync(excelStream);

        // Assert
        Assert.Equal(2, result.SuccessCount);
        var customerCount = await _context.Customers.CountAsync();
        Assert.Equal(1, customerCount); // Should only create one customer
    }

    [Fact]
    public async Task ImportDataAsync_WithMissingProductData_ShouldLogError()
    {
        // Arrange
        var excelStream = CreateTestExcelFile(new[]
        {
            new { Codigo = "", Producto = "", Precio = "25000", Stock = "100", 
                  Documento = "123456", Cliente = "Juan Perez", Email = "juan@email.com", Cantidad = "5" }
        });

        // Act
        var result = await _service.ImportDataAsync(excelStream);

        // Assert
        Assert.Equal(0, result.SuccessCount);
        Assert.Equal(1, result.ErrorCount);
        Assert.Contains("Missing mandatory product data", result.Errors[0]);
    }

    [Fact]
    public async Task ImportDataAsync_WithMissingCustomerData_ShouldLogError()
    {
        // Arrange
        var excelStream = CreateTestExcelFile(new[]
        {
            new { Codigo = "PROD001", Producto = "Cemento", Precio = "25000", Stock = "100", 
                  Documento = "", Cliente = "", Email = "", Cantidad = "5" }
        });

        // Act
        var result = await _service.ImportDataAsync(excelStream);

        // Assert
        Assert.Equal(0, result.SuccessCount);
        Assert.Equal(1, result.ErrorCount);
        Assert.Contains("Missing mandatory customer data", result.Errors[0]);
    }

    [Fact]
    public async Task ImportDataAsync_WithDuplicateProducts_ShouldReuseFromCache()
    {
        // Arrange
        var excelStream = CreateTestExcelFile(new[]
        {
            new { Codigo = "PROD001", Producto = "Cemento", Precio = "25000", Stock = "100", 
                  Documento = "123456", Cliente = "Juan Perez", Email = "juan@email.com", Cantidad = "5" },
            new { Codigo = "PROD001", Producto = "Cemento", Precio = "30000", Stock = "150", 
                  Documento = "789012", Cliente = "Maria Garcia", Email = "maria@email.com", Cantidad = "2" }
        });

        // Act
        var result = await _service.ImportDataAsync(excelStream);

        // Assert
        Assert.Equal(2, result.SuccessCount);
        var productCount = await _context.Products.CountAsync();
        Assert.Equal(1, productCount); // Should only create one product
        
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Code == "PROD001");
        Assert.NotNull(product);
        // First occurrence is cached and reused, so price remains 25000
        Assert.Equal(25000, product.Price);
    }

    [Fact]
    public async Task ImportDataAsync_WithMultipleRows_ShouldCreateSales()
    {
        // Arrange
        var excelStream = CreateTestExcelFile(new[]
        {
            new { Codigo = "PROD001", Producto = "Cemento", Precio = "25000", Stock = "100", 
                  Documento = "123456", Cliente = "Juan Perez", Email = "juan@email.com", Cantidad = "5" },
            new { Codigo = "PROD002", Producto = "Arena", Precio = "45000", Stock = "50", 
                  Documento = "789012", Cliente = "Maria Garcia", Email = "maria@email.com", Cantidad = "3" }
        });

        // Act
        var result = await _service.ImportDataAsync(excelStream);

        // Assert
        Assert.Equal(2, result.SuccessCount);
        var salesCount = await _context.Sales.CountAsync();
        Assert.Equal(2, salesCount);
    }

    private MemoryStream CreateTestExcelFile(dynamic[] rows)
    {
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Test");

        // Headers
        worksheet.Cells[1, 1].Value = "Codigo";
        worksheet.Cells[1, 2].Value = "Producto";
        worksheet.Cells[1, 3].Value = "Precio";
        worksheet.Cells[1, 4].Value = "Stock";
        worksheet.Cells[1, 5].Value = "Documento";
        worksheet.Cells[1, 6].Value = "Cliente";
        worksheet.Cells[1, 7].Value = "Email";
        worksheet.Cells[1, 8].Value = "Cantidad";

        // Data
        for (int i = 0; i < rows.Length; i++)
        {
            var row = rows[i];
            worksheet.Cells[i + 2, 1].Value = row.Codigo;
            worksheet.Cells[i + 2, 2].Value = row.Producto;
            worksheet.Cells[i + 2, 3].Value = row.Precio;
            worksheet.Cells[i + 2, 4].Value = row.Stock;
            worksheet.Cells[i + 2, 5].Value = row.Documento;
            worksheet.Cells[i + 2, 6].Value = row.Cliente;
            worksheet.Cells[i + 2, 7].Value = row.Email;
            worksheet.Cells[i + 2, 8].Value = row.Cantidad;
        }

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        return stream;
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}
