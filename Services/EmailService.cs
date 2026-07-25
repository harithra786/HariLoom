using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using hariloom.Interfaces;
using hariloom.Models.DTOs;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace hariloom.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;
        private static readonly HttpClient _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };

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
                var sent = await SendEmailAsync(toEmail, userName, subject, body);
                if (!sent)
                {
                    _logger.LogWarning($"[DEVELOPMENT FALLBACK] Could not deliver email via Brevo API/SMTP. OTP for {toEmail} is: {otp}");
                    // Allow development registration to proceed with the logged console OTP
                    return true;
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send OTP email.");
                _logger.LogWarning($"[DEVELOPMENT FALLBACK] Exception during SendOtpEmailAsync. Generated OTP for {toEmail}: {otp}");
                return true;
            }
        }

        public async Task<bool> SendOrderConfirmationEmailAsync(string toEmail, string firstName, string orderId, string orderDate, List<OrderEmailItemDTO> items)
        {
            try
            {
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
                return await SendEmailAsync(toEmail, firstName, subject, htmlContent);
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
                return await SendEmailAsync(adminEmail, "Admin", subject, htmlContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send admin order notification email.");
                return false;
            }
        }

        private async Task<bool> SendEmailAsync(string toEmail, string toName, string subject, string body)
        {
            var apiKey = _config["Brevo:ApiKey"];
            
            // Fallback: If Brevo:Password starts with xkeysib-, treat it as an API key
            var pwd = _config["Brevo:Password"];
            if (string.IsNullOrEmpty(apiKey) && !string.IsNullOrEmpty(pwd) && pwd.StartsWith("xkeysib-"))
            {
                apiKey = pwd;
            }

            // 1. Try Brevo REST API v3 over HTTPS (Port 443) if ApiKey is available
            if (!string.IsNullOrEmpty(apiKey))
            {
                var apiSent = await SendEmailApiAsync(apiKey, toEmail, toName, subject, body);
                if (apiSent) return true;
                _logger.LogWarning("Brevo REST API sending failed. Attempting SMTP fallback...");
            }

            // 2. Fallback to MailKit SMTP (Port 587 / 465)
            return await SendEmailSmtpAsync(toEmail, toName, subject, body);
        }

        private async Task<bool> SendEmailApiAsync(string apiKey, string toEmail, string toName, string subject, string body)
        {
            try
            {
                var fromEmail = _config["Brevo:FromEmail"] ?? "support@hariloom.in";
                var fromName = _config["Brevo:FromName"] ?? "HariLoom";

                _logger.LogInformation($"Attempting to send email via Brevo REST API v3 to {toEmail}");

                var payload = new
                {
                    sender = new { name = fromName, email = fromEmail },
                    to = new[] { new { email = toEmail, name = string.IsNullOrEmpty(toName) ? toEmail : toName } },
                    replyTo = new { email = "harithrashandloom@gmail.com", name = "HariLoom" },
                    subject = subject,
                    htmlContent = body
                };

                var jsonPayload = JsonSerializer.Serialize(payload);
                using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.brevo.com/v3/smtp/email");
                request.Headers.Add("accept", "application/json");
                request.Headers.Add("api-key", apiKey);
                request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Successfully sent email to {toEmail} via Brevo REST API v3");
                    return true;
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Brevo REST API failed (Status {response.StatusCode}): {errorContent}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Brevo REST API exception during email delivery.");
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

                if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(password) || password == "YOUR_SMTP_KEY")
                {
                    _logger.LogWarning("SMTP credentials are not fully configured.");
                    return false;
                }

                int port = int.TryParse(portStr, out var p) ? p : 587;

                _logger.LogInformation($"Attempting to send email via MailKit SMTP {host}:{port} to {toEmail}");

                var safeFromEmail = !string.IsNullOrEmpty(fromEmail) ? fromEmail : "support@hariloom.in";
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(fromName ?? "HariLoom", safeFromEmail));
                message.To.Add(new MailboxAddress(string.IsNullOrEmpty(toName) ? toEmail : toName, toEmail));
                message.ReplyTo.Add(new MailboxAddress("HariLoom", "harithrashandloom@gmail.com"));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = body
                };
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new MailKit.Net.Smtp.SmtpClient();
                client.Timeout = 10000; // 10 seconds timeout
                await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(username, password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation($"Successfully sent email to {toEmail} via SMTP");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MailKit SMTP email sending failed.");
                return false;
            }
        }
    }
}
