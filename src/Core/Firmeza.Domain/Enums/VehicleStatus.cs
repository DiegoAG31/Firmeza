namespace Firmeza.Domain.Enums;

/// <summary>
/// Status of industrial vehicles
/// </summary>
public enum VehicleStatus
{
    /// <summary>
    /// Vehicle is available for rent
    /// </summary>
    Available = 1,

    /// <summary>
    /// Vehicle is currently rented
    /// </summary>
    Rented = 2,

    /// <summary>
    /// Vehicle is under maintenance
    /// </summary>
    Maintenance = 3,

    /// <summary>
    /// Vehicle is out of service
    /// </summary>
    OutOfService = 4
}
