using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using hariloom.Interfaces;
using hariloom.Models.DTOs;

namespace hariloom.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task<bool> SendOtpEmailAsync(string toEmail, string userName, string otp)
        {
            try
            {
                var subject = "Your Hariloom OTP Code";
                var body = $"<html><body><p>Hi {userName},</p><p>Your OTP code is <strong>{otp}</strong>. It will expire in 5 minutes.</p><p>Thanks,<br/>Hariloom Team</p></body></html>";
                var sent = await SendEmailSmtpAsync(toEmail, userName, subject, body);
                if (!sent)
                {
                    _logger.LogWarning($"[DEVELOPMENT FALLBACK] Failed to send OTP email via SMTP. The generated OTP code is: {otp}");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send OTP email.");
                _logger.LogWarning($"[DEVELOPMENT FALLBACK] Exception during SendOtpEmailAsync. The generated OTP code is: {otp}");
                return false;
            }
        }

        public async Task<bool> SendOrderConfirmationEmailAsync(string toEmail, string firstName, string orderId, string orderDate, List<OrderEmailItemDTO> items)
        {
            try
            {
                var itemsHtml = "";
                foreach (var item in items)
                {
                    // Basic fallback for ImageUrl if empty. Replace or adjust as needed.
                    var imgTag = !string.IsNullOrEmpty(item.ImageUrl) ? $"<img src=\"{item.ImageUrl}\" width=\"50\" />" : "No Image";
                    itemsHtml += $"<tr><td>{imgTag}</td><td>{item.Name}</td><td>{item.Units}</td><td>₹{item.Price}</td></tr>";
                }

                var htmlContent = $@"
                <html>
                <body>
                    <div style='display:inline-block; width:60px; height:60px; border-radius:50%; background-color:#111; color:#fff; text-align:center; line-height:60px; font-family:sans-serif; font-weight:bold; font-size:14px; margin-bottom: 20px;'>Hariloom.</div>
                    <p>Hi {firstName},</p>
                    <br/>
                    <p>Your order has been successfully placed.</p>
                    <br/>
                    <p>Thoughtfully made and carefully packed, your pieces will be on their way soon.</p> 
                    <br/>
                    <p>We’ll send you shipping details once your order is dispatched.</p>
                    <br/>
                    <h3>Order Summary</h3>
                    <p><strong>Order #:</strong> {orderId}<br/>
                    <strong>Order Date:</strong> {orderDate}</p>
                    <br/>
                    <table border='1' cellpadding='5' cellspacing='0' style='border-collapse: collapse;'>
                        <tr style='background-color: #f2f2f2;'><th>Image</th><th>Product Name</th><th>Quantity</th><th>Price</th></tr>
                        {itemsHtml}
                    </table>
                    <br/>
                    <p>Thank you for shopping Hariloom.</p>
                    <br/>
                    <p>Team Hariloom 🤎</p>
                </body>
                </html>";

                var subject = "Order confirmed — thank you for choosing Hariloom.";
                return await SendEmailSmtpAsync(toEmail, firstName, subject, htmlContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send order confirmation email.");
                return false;
            }
        }

        public async Task<bool> SendNewOrderAdminNotificationAsync(string orderNumber, string totalAmount, List<OrderEmailItemDTO> items)
        {
            try
            {
                var adminEmail = _config["Brevo:AdminEmail"] ?? _config["Brevo:FromEmail"];
                if (string.IsNullOrEmpty(adminEmail)) return false;

                var itemsHtml = "";
                foreach (var item in items)
                {
                    var imgTag = !string.IsNullOrEmpty(item.ImageUrl) ? $"<img src=\"{item.ImageUrl}\" width=\"50\" />" : "No Image";
                    itemsHtml += $"<tr><td>{imgTag}</td><td>{item.Name}</td><td>{item.Units}</td><td>₹{item.Price}</td></tr>";
                }

                var htmlContent = $@"
                <html>
                <body>
                    <div style='display:inline-block; width:60px; height:60px; border-radius:50%; background-color:#111; color:#fff; text-align:center; line-height:60px; font-family:sans-serif; font-weight:bold; font-size:14px; margin-bottom: 20px;'>Hariloom.</div>
                    <h2>New Order Received</h2>
                    <p>A new order has been placed successfully.</p>
                    <p><strong>Order ID:</strong> {orderNumber}</p>
                    <p><strong>Total Amount:</strong> ₹{totalAmount}</p>
                    <h3>Order Items:</h3>
                    <table border='1' cellpadding='5' cellspacing='0'>
                        <tr><th>Image</th><th>Product Name</th><th>Quantity</th><th>Price</th></tr>
                        {itemsHtml}
                    </table>
                </body>
                </html>";

                var subject = $"New Order Notification - {orderNumber}";
                return await SendEmailSmtpAsync(adminEmail, "Admin", subject, htmlContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send admin order notification email.");
                return false;
            }
        }

        private async Task<bool> SendEmailSmtpAsync(string toEmail, string toName, string subject, string body)
        {
            try
            {
                var host = _config["Brevo:Host"];
                var portStr = _config["Brevo:Port"];
                var username = _config["Brevo:Username"];
                var password = _config["Brevo:Password"];
                var fromEmail = _config["Brevo:FromEmail"];
                var fromName = _config["Brevo:FromName"];

                if (string.IsNullOrEmpty(host) || password == "YOUR_SMTP_KEY")
                {
                    _logger.LogWarning("SMTP credentials are not fully configured.");
                    return true; // Pretend it succeeds for development without key
                }

                int port = int.TryParse(portStr, out var p) ? p : 587;

                _logger.LogInformation($"Attempting to send email via SMTP {host}:{port} to {toEmail}");

                using var message = new MailMessage();
                message.From = new MailAddress(fromEmail, fromName);
                message.ReplyToList.Add(
                    new MailAddress("harithrashandloom@gmail.com", "HariLoom")
                );
                if (string.IsNullOrEmpty(toName))
                {
                    message.To.Add(new MailAddress(toEmail));
                }
                else
                {
                    message.To.Add(new MailAddress(toEmail, toName));
                }
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;

                using var client = new SmtpClient(host, port);
                client.Credentials = new NetworkCredential(username, password);
                client.EnableSsl = true;

                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
                await client.SendMailAsync(message, cts.Token);
                _logger.LogInformation($"Successfully sent email to {toEmail}");
                return true;
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "SMTP Email sending timed out. DigitalOcean often blocks port 587. Try changing Brevo:Port to 2525 or 465 in appsettings.json.");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SMTP Email sending failed.");
                return false;
            }
        }
    }
}
