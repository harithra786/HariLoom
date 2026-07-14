using System.Collections.Generic;

namespace hariloom.Models.DTOs
{
    public class SendOtpRequestDTO
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Otp { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class SendOrderConfirmationRequestDTO
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string OrderId { get; set; }
        public string OrderDate { get; set; }
        public string TotalAmount { get; set; }
        public List<OrderEmailItemDTO> Items { get; set; }
    }
}
