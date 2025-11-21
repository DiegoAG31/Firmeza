namespace Firmeza.Domain.Enums;

/// <summary>
/// Status of sales transactions
/// </summary>
public enum SaleStatus
{
    /// <summary>
    /// Sale is pending completion
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Sale has been completed successfully
    /// </summary>
    Completed = 2,

    /// <summary>
    /// Sale has been cancelled
    /// </summary>
    Cancelled = 3
}
