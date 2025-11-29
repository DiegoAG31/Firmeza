namespace Firmeza.Application.Settings;

/// <summary>
/// Email configuration settings
/// </summary>
public class EmailSettings
{
    /// <summary>
    /// SMTP server address (e.g., smtp.gmail.com)
    /// </summary>
    public string SmtpServer { get; set; } = string.Empty;

    /// <summary>
    /// SMTP server port (e.g., 587 for TLS)
    /// </summary>
    public int SmtpPort { get; set; }

    /// <summary>
    /// Sender email address
    /// </summary>
    public string SenderEmail { get; set; } = string.Empty;

    /// <summary>
    /// Sender display name
    /// </summary>
    public string SenderName { get; set; } = string.Empty;

    /// <summary>
    /// SMTP username (usually same as sender email)
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// SMTP password or app password
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Enable SSL/TLS encryption
    /// </summary>
    public bool EnableSsl { get; set; } = true;
}
