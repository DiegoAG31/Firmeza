using Firmeza.Domain.Common;

namespace Firmeza.Domain.Entities;

/// <summary>
/// Represents a customer in the system
/// </summary>
public class Customer : BaseEntity
{
    /// <summary>
    /// Customer's first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Customer's last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Full name (computed property)
    /// </summary>
    public string FullName => $"{FirstName} {LastName}";

    /// <summary>
    /// Document type (CC, NIT, CE, etc.)
    /// </summary>
    public string DocumentType { get; set; } = string.Empty;

    /// <summary>
    /// Document number (unique identifier)
    /// </summary>
    public string DocumentNumber { get; set; } = string.Empty;

    /// <summary>
    /// Customer's email address
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Customer's phone number
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// Optional physical address
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// City where the customer is located
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Optional reference to Identity User (if customer has login)
    /// </summary>
    public string? UserId { get; set; }

    // Navigation properties
    /// <summary>
    /// Sales made by this customer
    /// </summary>
    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();

    /// <summary>
    /// Vehicle rentals by this customer
    /// </summary>
    public virtual ICollection<VehicleRental> Rentals { get; set; } = new List<VehicleRental>();
}
