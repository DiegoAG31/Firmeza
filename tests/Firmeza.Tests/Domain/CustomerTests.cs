using Firmeza.Domain.Entities;

namespace Firmeza.Tests.Domain;

public class CustomerTests
{
    [Fact]
    public void Customer_Creation_WithValidData_ShouldSucceed()
    {
        // Arrange & Act
        var customer = new Customer
        {
            FirstName = "Juan",
            LastName = "Pérez",
            DocumentType = "CC",
            DocumentNumber = "1234567890",
            Email = "juan.perez@email.com",
            Phone = "3001234567",
            IsActive = true
        };

        // Assert
        Assert.Equal("Juan", customer.FirstName);
        Assert.Equal("Pérez", customer.LastName);
        Assert.Equal("1234567890", customer.DocumentNumber);
        Assert.Equal("juan.perez@email.com", customer.Email);
        Assert.True(customer.IsActive);
    }

    [Fact]
    public void Customer_FullName_ShouldCombineFirstAndLastName()
    {
        // Arrange
        var customer = new Customer
        {
            FirstName = "María",
            LastName = "García"
        };

        // Act
        var fullName = customer.FullName;

        // Assert
        Assert.Equal("María García", fullName);
    }

    [Fact]
    public void Customer_Email_ShouldBeValid()
    {
        // Arrange
        var customer = new Customer
        {
            FirstName = "Carlos",
            LastName = "López",
            DocumentNumber = "1122334455",
            Email = "carlos.lopez@email.com"
        };

        // Assert
        Assert.Contains("@", customer.Email);
        Assert.Contains(".", customer.Email);
    }

    [Theory]
    [InlineData("CC")]
    [InlineData("NIT")]
    [InlineData("CE")]
    [InlineData("PA")]
    public void Customer_DocumentType_ShouldAcceptValidTypes(string documentType)
    {
        // Arrange & Act
        var customer = new Customer
        {
            FirstName = "Test",
            LastName = "User",
            DocumentType = documentType,
            DocumentNumber = "123456"
        };

        // Assert
        Assert.Equal(documentType, customer.DocumentType);
    }

    [Fact]
    public void Customer_WithOptionalFields_ShouldAllowNulls()
    {
        // Arrange & Act
        var customer = new Customer
        {
            FirstName = "Ana",
            LastName = "Martínez",
            DocumentNumber = "5566778899",
            Email = "ana@email.com",
            Phone = "3009876543",
            Address = null,
            City = null
        };

        // Assert
        Assert.Null(customer.Address);
        Assert.Null(customer.City);
    }
}
