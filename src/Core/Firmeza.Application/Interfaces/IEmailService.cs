namespace Firmeza.Application.Interfaces;

/// <summary>
/// Service for sending emails
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Send a generic email
    /// </summary>
    Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);

    /// <summary>
    /// Send a generic email with attachments
    /// </summary>
    Task SendEmailAsync(string to, string subject, string body, byte[] attachment, string attachmentName, bool isHtml = true);

    /// <summary>
    /// Send welcome email to new user
    /// </summary>
    Task SendWelcomeEmailAsync(string to, string customerName);

    /// <summary>
    /// Send purchase confirmation email with PDF receipt
    /// </summary>
    Task SendPurchaseConfirmationAsync(string to, string customerName, string saleNumber, decimal total, byte[]? pdfReceipt = null);
}
