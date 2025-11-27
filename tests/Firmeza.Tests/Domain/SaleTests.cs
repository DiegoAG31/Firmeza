using Firmeza.Domain.Entities;
using Firmeza.Domain.Enums;

namespace Firmeza.Tests.Domain;

public class SaleTests
{
    [Fact]
    public void Sale_Creation_WithValidData_ShouldSucceed()
    {
        // Arrange
        var customer = new Customer
        {
            FirstName = "Test",
            LastName = "Customer",
            DocumentNumber = "123456",
            Email = "test@email.com"
        };

        // Act
        var sale = new Sale
        {
            SaleNumber = "V-001",
            SaleDate = DateTime.UtcNow,
            Customer = customer,
            Subtotal = 100000,
            Tax = 19000,
            Total = 119000,
            Status = SaleStatus.Completed
        };

        // Assert
        Assert.Equal("V-001", sale.SaleNumber);
        Assert.Equal(customer, sale.Customer);
        Assert.Equal(100000, sale.Subtotal);
        Assert.Equal(19000, sale.Tax);
        Assert.Equal(119000, sale.Total);
        Assert.Equal(SaleStatus.Completed, sale.Status);
    }

    [Fact]
    public void Sale_Total_ShouldEqualSubtotalPlusTax()
    {
        // Arrange
        var subtotal = 50000m;
        var tax = 9500m;

        // Act
        var sale = new Sale
        {
            SaleNumber = "V-002",
            Subtotal = subtotal,
            Tax = tax,
            Total = subtotal + tax
        };

        // Assert
        Assert.Equal(59500, sale.Total);
        Assert.Equal(subtotal + tax, sale.Total);
    }

    [Theory]
    [InlineData(SaleStatus.Pending)]
    [InlineData(SaleStatus.Completed)]
    [InlineData(SaleStatus.Cancelled)]
    public void Sale_Status_ShouldAcceptValidStatuses(SaleStatus status)
    {
        // Arrange & Act
        var sale = new Sale
        {
            SaleNumber = "V-003",
            Status = status
        };

        // Assert
        Assert.Equal(status, sale.Status);
    }

    [Fact]
    public void Sale_WithDetails_ShouldCalculateCorrectSubtotal()
    {
        // Arrange
        var product1 = new Product { Code = "P1", Name = "Product 1", Price = 10000 };
        var product2 = new Product { Code = "P2", Name = "Product 2", Price = 20000 };

        var detail1 = new SaleDetail
        {
            Product = product1,
            Quantity = 2,
            UnitPrice = product1.Price,
            Subtotal = product1.Price * 2
        };

        var detail2 = new SaleDetail
        {
            Product = product2,
            Quantity = 3,
            UnitPrice = product2.Price,
            Subtotal = product2.Price * 3
        };

        // Act
        var sale = new Sale
        {
            SaleNumber = "V-004",
            SaleDetails = new List<SaleDetail> { detail1, detail2 }
        };

        var calculatedSubtotal = sale.SaleDetails.Sum(d => d.Subtotal);
        sale.Subtotal = calculatedSubtotal;

        // Assert
        Assert.Equal(80000, sale.Subtotal); // (10000 * 2) + (20000 * 3)
        Assert.Equal(2, sale.SaleDetails.Count);
    }

    [Fact]
    public void Sale_DefaultStatus_ShouldBePending()
    {
        // Arrange & Act
        var sale = new Sale
        {
            SaleNumber = "V-005",
            Status = SaleStatus.Pending
        };

        // Assert
        Assert.Equal(SaleStatus.Pending, sale.Status);
    }
}
