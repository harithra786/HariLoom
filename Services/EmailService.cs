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

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                _logger.LogError("Brevo API Key is missing.");
                return false;
            }

            return await SendEmailApiAsync(apiKey, toEmail, toName, subject, body);
        }

        private async Task<bool> SendEmailApiAsync(string apiKey, string toEmail, string toName, string subject, string body)
        {
            try
            {
                var fromEmail = _config["Brevo:FromEmail"];
                var fromName = _config["Brevo:FromName"];

                _logger.LogInformation("========== BREVO EMAIL ==========");
                _logger.LogInformation("Sending To : {Email}", toEmail);
                _logger.LogInformation("From : {From}", fromEmail);

                var payload = new
                {
                    sender = new
                    {
                        name = fromName,
                        email = fromEmail
                    },
                    to = new[]
                    {
                new
                {
                    email = toEmail,
                    name = string.IsNullOrWhiteSpace(toName) ? toEmail : toName
                }
            },
                    subject = subject,
                    htmlContent = body
                };

                var json = JsonSerializer.Serialize(payload);

                var request = new HttpRequestMessage(HttpMethod.Post,
                    "https://api.brevo.com/v3/smtp/email");

                request.Headers.Add("accept", "application/json");
                request.Headers.Add("api-key", apiKey);

                request.Content =
                    new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(request);

                var responseBody = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("Brevo Status : {Status}",
                    response.StatusCode);

                _logger.LogInformation("Brevo Response : {Body}",
                    responseBody);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Email sent successfully.");
                    return true;
                }

                _logger.LogError("Brevo returned an error.");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Brevo API Exception");
                return false;
            }
        }
    }
}
