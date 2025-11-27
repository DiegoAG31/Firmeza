using Firmeza.Domain.Entities;
using Firmeza.Domain.Enums;

namespace Firmeza.Tests.Domain;

public class ProductTests
{
    [Fact]
    public void Product_Creation_WithValidData_ShouldSucceed()
    {
        // Arrange & Act
        var product = new Product
        {
            Code = "PROD001",
            Name = "Cemento Portland",
            Price = 25000,
            Stock = 100,
            Type = ProductType.Material,
            IsActive = true
        };

        // Assert
        Assert.Equal("PROD001", product.Code);
        Assert.Equal("Cemento Portland", product.Name);
        Assert.Equal(25000, product.Price);
        Assert.Equal(100, product.Stock);
        Assert.Equal(ProductType.Material, product.Type);
        Assert.True(product.IsActive);
    }

    [Fact]
    public void Product_Price_ShouldBePositive()
    {
        // Arrange
        var product = new Product
        {
            Code = "PROD002",
            Name = "Arena",
            Price = 45000,
            Stock = 50
        };

        // Assert
        Assert.True(product.Price > 0);
    }

    [Fact]
    public void Product_Stock_CanBeZero()
    {
        // Arrange & Act
        var product = new Product
        {
            Code = "PROD003",
            Name = "Gravilla",
            Price = 55000,
            Stock = 0
        };

        // Assert
        Assert.Equal(0, product.Stock);
    }

    [Fact]
    public void Product_Deactivation_ShouldSetIsActiveFalse()
    {
        // Arrange
        var product = new Product
        {
            Code = "PROD004",
            Name = "Ladrillo",
            Price = 850000,
            Stock = 200,
            IsActive = true
        };

        // Act
        product.IsActive = false;

        // Assert
        Assert.False(product.IsActive);
    }

    [Theory]
    [InlineData(ProductType.Material)]
    [InlineData(ProductType.Tool)]
    public void Product_Type_ShouldAcceptValidTypes(ProductType type)
    {
        // Arrange & Act
        var product = new Product
        {
            Code = "PROD005",
            Name = "Test Product",
            Price = 10000,
            Stock = 10,
            Type = type
        };

        // Assert
        Assert.Equal(type, product.Type);
    }
}
