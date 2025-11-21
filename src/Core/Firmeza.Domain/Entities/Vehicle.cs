using Firmeza.Domain.Common;
using Firmeza.Domain.Enums;

namespace Firmeza.Domain.Entities;

/// <summary>
/// Represents an industrial vehicle available for rent
/// </summary>
public class Vehicle : BaseEntity
{
    /// <summary>
    /// Vehicle brand (e.g., "Caterpillar", "Volvo")
    /// </summary>
    public string Brand { get; set; } = string.Empty;

    /// <summary>
    /// Vehicle model
    /// </summary>
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// License plate number (unique identifier)
    /// </summary>
    public string PlateNumber { get; set; } = string.Empty;

    /// <summary>
    /// Manufacturing year
    /// </summary>
    public int Year { get; set; }

    /// <summary>
    /// Daily rental price
    /// </summary>
    public decimal PricePerDay { get; set; }

    /// <summary>
    /// Current status of the vehicle
    /// </summary>
    public VehicleStatus Status { get; set; } = VehicleStatus.Available;

    /// <summary>
    /// Optional image URL
    /// </summary>
    public string? ImageUrl { get; set; }

    // Navigation properties
    /// <summary>
    /// Rental history for this vehicle
    /// </summary>
    public virtual ICollection<VehicleRental> Rentals { get; set; } = new List<VehicleRental>();
}
