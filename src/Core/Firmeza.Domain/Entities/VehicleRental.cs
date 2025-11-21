using Firmeza.Domain.Common;

namespace Firmeza.Domain.Entities;

/// <summary>
/// Represents a vehicle rental transaction
/// </summary>
public class VehicleRental : BaseEntity
{
    /// <summary>
    /// Unique rental number (e.g., RV-00001)
    /// </summary>
    public string RentalNumber { get; set; } = string.Empty;

    /// <summary>
    /// Foreign key to Vehicle
    /// </summary>
    public int VehicleId { get; set; }

    /// <summary>
    /// Foreign key to Customer
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Rental start date
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Rental end date
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Number of rental days
    /// </summary>
    public int Days { get; set; }

    /// <summary>
    /// Total rental amount
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Indicates if the vehicle has been returned
    /// </summary>
    public bool IsReturned { get; set; }

    /// <summary>
    /// Actual return date
    /// </summary>
    public DateTime? ReturnDate { get; set; }

    // Navigation properties
    /// <summary>
    /// Vehicle being rented
    /// </summary>
    public virtual Vehicle Vehicle { get; set; } = null!;

    /// <summary>
    /// Customer renting the vehicle
    /// </summary>
    public virtual Customer Customer { get; set; } = null!;
}
