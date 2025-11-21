using Firmeza.Domain.Common;

namespace Firmeza.Domain.Entities;

/// <summary>
/// Represents individual line items in a sale
/// </summary>
public class SaleDetail : BaseEntity
{
    /// <summary>
    /// Foreign key to Sale
    /// </summary>
    public int SaleId { get; set; }

    /// <summary>
    /// Foreign key to Product
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Quantity of the product sold
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Unit price at the time of sale
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Subtotal for this line item (Quantity * UnitPrice)
    /// </summary>
    public decimal Subtotal { get; set; }

    // Navigation properties
    /// <summary>
    /// Sale this detail belongs to
    /// </summary>
    public virtual Sale Sale { get; set; } = null!;

    /// <summary>
    /// Product being sold
    /// </summary>
    public virtual Product Product { get; set; } = null!;
}
