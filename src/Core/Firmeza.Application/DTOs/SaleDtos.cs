using System.ComponentModel.DataAnnotations;

namespace Firmeza.Application.DTOs;

public class CreateSaleDto
{
    [Required]
    public int CustomerId { get; set; }

    [Required]
    public List<SaleItemDto> Items { get; set; } = new();
}

public class SaleItemDto
{
    [Required]
    public int ProductId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}

public class SaleResponseDto
{
    public int Id { get; set; }
    public string SaleNumber { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? PdfPath { get; set; }
}
