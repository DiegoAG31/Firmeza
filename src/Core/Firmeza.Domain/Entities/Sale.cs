using Firmeza.Domain.Common;
using Firmeza.Domain.Enums;

namespace Firmeza.Domain.Entities;

/// <summary>
/// Represents a sale transaction
/// </summary>
public class Sale : BaseEntity
{
    /// <summary>
    /// Unique sale number (e.g., FV-00001)
    /// </summary>
    public string SaleNumber { get; set; } = string.Empty;

    /// <summary>
    /// Date and time of the sale
    /// </summary>
    public DateTime SaleDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Foreign key to Customer
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Subtotal before taxes
    /// </summary>
    public decimal Subtotal { get; set; }

    /// <summary>
    /// Tax amount (IVA - typically 19% in Colombia)
    /// </summary>
    public decimal Tax { get; set; }

    /// <summary>
    /// Total amount (Subtotal + Tax)
    /// </summary>
    public decimal Total { get; set; }

    /// <summary>
    /// Status of the sale
    /// </summary>
    public SaleStatus Status { get; set; } = SaleStatus.Pending;

    /// <summary>
    /// Path to the generated PDF receipt
    /// </summary>
    public string? PdfPath { get; set; }

    /// <summary>
    /// Additional notes or observations about the sale
    /// </summary>
    public string? Observations { get; set; }

    // Navigation properties
    /// <summary>
    /// Customer who made the purchase
    /// </summary>
    public virtual Customer Customer { get; set; } = null!;

    /// <summary>
    /// Items included in this sale
    /// </summary>
    public virtual ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();
}
