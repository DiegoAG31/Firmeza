using Firmeza.Application.DTOs;
using Firmeza.Application.Interfaces;
using Firmeza.Domain.Entities;
using Firmeza.Domain.Enums;
using Firmeza.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Firmeza.Infrastructure.Services;

public class ExcelImportService : IExcelImportService
{
    private readonly ApplicationDbContext _context;

    public ExcelImportService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ImportResult> ImportDataAsync(Stream fileStream)
    {
        // Configure EPPlus License
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        
        var result = new ImportResult();
        using var package = new ExcelPackage(fileStream);
        var worksheet = package.Workbook.Worksheets[0];
        var rowCount = worksheet.Dimension.Rows;

        // Assume headers are in row 1
        // Mapping columns by name would be better, but for now we'll try to identify by header or fixed position
        // Let's try to find column indices by header name
        var headers = new Dictionary<string, int>();
        for (int col = 1; col <= worksheet.Dimension.Columns; col++)
        {
            var header = worksheet.Cells[1, col].Text.ToLower().Trim();
            if (!string.IsNullOrEmpty(header))
            {
                headers[header] = col;
            }
        }

        // Track processed entities to avoid duplicates within the same import
        var processedCustomers = new Dictionary<string, Customer>();
        var processedProducts = new Dictionary<string, Product>();

        for (int row = 2; row <= rowCount; row++)
        {
            try
            {
                // 1. Extract and Normalize Product
                var product = await ProcessProductAsync(worksheet, row, headers, processedProducts);
                if (product == null)
                {
                    result.Errors.Add($"Row {row}: Missing mandatory product data (Code or Name).");
                    result.ErrorCount++;
                    continue;
                }

                // 2. Extract and Normalize Customer
                var customer = await ProcessCustomerAsync(worksheet, row, headers, processedCustomers);
                if (customer == null)
                {
                    result.Errors.Add($"Row {row}: Missing mandatory customer data (Document or Email).");
                    result.ErrorCount++;
                    continue;
                }

                // 3. Create Sale
                ProcessSaleAsync(worksheet, row, headers, product, customer);
                
                // Save changes after each row to avoid duplicate key issues
                await _context.SaveChangesAsync();
                
                result.SuccessCount++;
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Row {row}: Unexpected error - {ex.Message}");
                result.ErrorCount++;
            }
        }

        return result;
    }

    private async Task<Product?> ProcessProductAsync(ExcelWorksheet worksheet, int row, Dictionary<string, int> headers, Dictionary<string, Product> processedProducts)
    {
        var code = GetValue(worksheet, row, headers, "codigo", "code", "sku");
        var name = GetValue(worksheet, row, headers, "producto", "product", "nombre", "name");
        
        if (string.IsNullOrEmpty(code) && string.IsNullOrEmpty(name)) return null;

        var priceStr = GetValue(worksheet, row, headers, "precio", "price");
        decimal.TryParse(priceStr, out var price);

        var stockStr = GetValue(worksheet, row, headers, "stock", "cantidad", "quantity"); // Stock or Quantity available
        int.TryParse(stockStr, out var stock);

        Product? product = null;
        
        // Check cache first
        if (!string.IsNullOrEmpty(code) && processedProducts.TryGetValue(code, out product))
        {
            return product;
        }
        
        // Check database
        if (!string.IsNullOrEmpty(code))
        {
            product = await _context.Products.FirstOrDefaultAsync(p => p.Code == code);
        }
        
        if (product == null && !string.IsNullOrEmpty(name))
        {
             product = await _context.Products.FirstOrDefaultAsync(p => p.Name == name);
        }

        if (product == null)
        {
            product = new Product
            {
                Code = code ?? Guid.NewGuid().ToString().Substring(0, 8),
                Name = name ?? "Unknown Product",
                Price = price,
                Stock = stock,
                Type = ProductType.Material, // Default
                IsActive = true
            };
            _context.Products.Add(product);
            
            // Add to cache
            if (!string.IsNullOrEmpty(code))
            {
                processedProducts[code] = product;
            }
        }
        else
        {
            // Update existing
            if (price > 0) product.Price = price;
            product.Stock = stock > 0 ? stock : product.Stock;
            
            // Add to cache
            if (!string.IsNullOrEmpty(code) && !processedProducts.ContainsKey(code))
            {
                processedProducts[code] = product;
            }
        }

        return product;
    }

    private async Task<Customer?> ProcessCustomerAsync(ExcelWorksheet worksheet, int row, Dictionary<string, int> headers, Dictionary<string, Customer> processedCustomers)
    {
        var doc = GetValue(worksheet, row, headers, "documento", "document", "cedula", "nit");
        var email = GetValue(worksheet, row, headers, "email", "correo");
        var name = GetValue(worksheet, row, headers, "cliente", "customer", "nombre cliente");

        if (string.IsNullOrEmpty(doc) && string.IsNullOrEmpty(email)) return null;

        Customer? customer = null;
        
        // Check cache first
        if (!string.IsNullOrEmpty(doc) && processedCustomers.TryGetValue(doc, out customer))
        {
            return customer;
        }
        
        // Check database
        if (!string.IsNullOrEmpty(doc))
        {
            customer = await _context.Customers.FirstOrDefaultAsync(c => c.DocumentNumber == doc);
        }
        
        if (customer == null && !string.IsNullOrEmpty(email))
        {
            customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == email);
        }

        if (customer == null)
        {
            var names = (name ?? "Unknown Customer").Split(' ');
            var firstName = names[0];
            var lastName = names.Length > 1 ? string.Join(" ", names.Skip(1)) : "";

            customer = new Customer
            {
                DocumentNumber = doc ?? "N/A",
                DocumentType = "CC", // Default
                Email = email ?? $"noemail-{Guid.NewGuid()}@example.com",
                FirstName = firstName,
                LastName = lastName,
                Phone = "N/A",
                IsActive = true
            };
            _context.Customers.Add(customer);
            
            // Add to cache
            if (!string.IsNullOrEmpty(doc))
            {
                processedCustomers[doc] = customer;
            }
        }
        else
        {
            // Update info if provided
            if (!string.IsNullOrEmpty(name))
            {
                 var names = name.Split(' ');
                 customer.FirstName = names[0];
                 customer.LastName = names.Length > 1 ? string.Join(" ", names.Skip(1)) : "";
            }
            
            // Add to cache
            if (!string.IsNullOrEmpty(doc) && !processedCustomers.ContainsKey(doc))
            {
                processedCustomers[doc] = customer;
            }
        }

        return customer;
    }

    private void ProcessSaleAsync(ExcelWorksheet worksheet, int row, Dictionary<string, int> headers, Product product, Customer customer)
    {
        // Check if this row represents a sale
        var qtyStr = GetValue(worksheet, row, headers, "cantidad", "quantity", "qty");
        if (string.IsNullOrEmpty(qtyStr)) return; // No sale info

        if (int.TryParse(qtyStr, out var quantity) && quantity > 0)
        {
            // Create a new sale for this row
            // In a real scenario, we might group by a SaleNumber column.
            // Here we'll create individual sales for simplicity as per "datos desorganizados".
            
            var sale = new Sale
            {
                SaleNumber = $"IMP-{DateTime.Now.Ticks}-{row}",
                SaleDate = DateTime.UtcNow,
                Customer = customer,
                Status = SaleStatus.Completed,
                Tax = 0, // Simplified
            };

            var detail = new SaleDetail
            {
                Product = product,
                Quantity = quantity,
                UnitPrice = product.Price,
                Subtotal = product.Price * quantity
            };
            
            sale.SaleDetails.Add(detail);
            sale.Subtotal = detail.Subtotal;
            sale.Total = sale.Subtotal;

            _context.Sales.Add(sale);
            
            // Deduct stock?
            // product.Stock -= quantity; 
        }
    }

    private string? GetValue(ExcelWorksheet worksheet, int row, Dictionary<string, int> headers, params string[] aliases)
    {
        foreach (var alias in aliases)
        {
            if (headers.TryGetValue(alias, out var col))
            {
                var val = worksheet.Cells[row, col].Text;
                if (!string.IsNullOrWhiteSpace(val)) return val.Trim();
            }
        }
        return null;
    }
}
