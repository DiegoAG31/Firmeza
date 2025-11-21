using Firmeza.Domain.Common;
using Firmeza.Domain.Enums;

namespace Firmeza.Domain.Entities;

/// <summary>
/// Represents a construction product or tool available for sale
/// </summary>
public class Product : BaseEntity
{
    /// <summary>
    /// Product name (e.g., "Portland Cement", "Power Drill")
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Detailed product description
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Unique product SKU code
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Unit price of the product
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Current stock quantity
    /// </summary>
    public int Stock { get; set; }

    /// <summary>
    /// Product type (Material or Tool)
    /// </summary>
    public ProductType Type { get; set; }

    /// <summary>
    /// Optional image URL
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Foreign key to Category
    /// </summary>
    public int? CategoryId { get; set; }

    // Navigation properties
    /// <summary>
    /// Category this product belongs to
    /// </summary>
    public virtual Category? Category { get; set; }

    /// <summary>
    /// Sale details where this product appears
    /// </summary>
    public virtual ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();
}
