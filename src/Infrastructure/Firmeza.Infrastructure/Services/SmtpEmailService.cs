using System.Net;
using System.Net.Mail;
using Firmeza.Application.Interfaces;
using Firmeza.Application.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Firmeza.Infrastructure.Services;

/// <summary>
/// SMTP-based email service implementation
/// </summary>
public class SmtpEmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(IOptions<EmailSettings> emailSettings, ILogger<SmtpEmailService> logger)
    {
        _emailSettings = emailSettings.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
    {
        try
        {
            using var message = new MailMessage
            {
                From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            message.To.Add(to);

            using var smtpClient = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
            {
                Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
                EnableSsl = _emailSettings.EnableSsl
            };

            await smtpClient.SendMailAsync(message);
            _logger.LogInformation("Email sent successfully to {To} with subject: {Subject}", to, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To} with subject: {Subject}", to, subject);
            throw;
        }
    }

    public async Task SendEmailAsync(string to, string subject, string body, byte[] attachment, string attachmentName, bool isHtml = true)
    {
        try
        {
            using var message = new MailMessage
            {
                From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            message.To.Add(to);

            // Add attachment
            using var attachmentStream = new MemoryStream(attachment);
            var mailAttachment = new Attachment(attachmentStream, attachmentName, "application/pdf");
            message.Attachments.Add(mailAttachment);

            using var smtpClient = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
            {
                Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
                EnableSsl = _emailSettings.EnableSsl
            };

            await smtpClient.SendMailAsync(message);
            _logger.LogInformation("Email with attachment sent successfully to {To} with subject: {Subject}", to, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email with attachment to {To} with subject: {Subject}", to, subject);
            throw;
        }
    }

    public async Task SendWelcomeEmailAsync(string to, string customerName)
    {
        var subject = "¡Bienvenido a Firmeza!";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2 style='color: #d97706;'>¡Bienvenido a Firmeza, {customerName}!</h2>
                <p>Gracias por registrarte en nuestra plataforma.</p>
                <p>Ahora puedes disfrutar de todos nuestros productos y servicios.</p>
                <p>Si tienes alguna pregunta, no dudes en contactarnos.</p>
                <br>
                <p>Saludos,<br><strong>El equipo de Firmeza</strong></p>
            </body>
            </html>";

        await SendEmailAsync(to, subject, body, isHtml: true);
    }

    public async Task SendPurchaseConfirmationAsync(string to, string customerName, string saleNumber, decimal total, byte[]? pdfReceipt = null)
    {
        var subject = $"Confirmación de Compra - {saleNumber}";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2 style='color: #d97706;'>Confirmación de Compra</h2>
                <p>Hola {customerName},</p>
                <p>Tu compra ha sido procesada exitosamente.</p>
                <div style='background-color: #f3f4f6; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                    <p><strong>Número de Venta:</strong> {saleNumber}</p>
                    <p><strong>Total:</strong> ${total:N2}</p>
                </div>
                <p>{(pdfReceipt != null ? "Adjunto encontrarás tu comprobante de compra en formato PDF." : "Puedes descargar tu recibo desde tu cuenta.")}</p>
                <p>¡Gracias por tu compra!</p>
                <br>
                <p>Saludos,<br><strong>El equipo de Firmeza</strong></p>
            </body>
            </html>";

        if (pdfReceipt != null)
        {
            await SendEmailAsync(to, subject, body, pdfReceipt, $"Comprobante_{saleNumber}.pdf", isHtml: true);
        }
        else
        {
            await SendEmailAsync(to, subject, body, isHtml: true);
        }
    }
}
