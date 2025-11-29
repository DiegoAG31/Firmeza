using Firmeza.Domain.Enums;

namespace Firmeza.Application.DTOs;

public class SaleDto
{
    public int Id { get; set; }
    public string SaleNumber { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal SubTotal { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }
    public SaleStatus Status { get; set; }
    public string? Observations { get; set; }
    public List<SaleDetailDto> SaleDetails { get; set; } = new();
}

public class SaleDetailDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductCode { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal SubTotal { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }
}

public class CreateSaleDto
{
    public int CustomerId { get; set; }
    public string? Observations { get; set; }
    public List<CreateSaleDetailDto> Details { get; set; } = new();
}

public class CreateSaleDetailDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
