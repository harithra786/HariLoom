using System.Collections.Generic;
using System.Threading.Tasks;
using hariloom.Models.DTOs;

namespace hariloom.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendOtpEmailAsync(string toEmail, string userName, string otp);
        Task<bool> SendOrderConfirmationEmailAsync(string toEmail, string firstName, string orderId, string orderDate, List<OrderEmailItemDTO> items);
        Task<bool> SendNewOrderAdminNotificationAsync(string orderNumber, string totalAmount, string customerName, string customerPhone, string customerAddress, List<OrderEmailItemDTO> items);
    }
}
