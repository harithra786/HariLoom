using System.Collections.Generic;

namespace hariloom.Models.DTOs
{
    public class OrderDashboardDTO
    {
        public int orderId { get; set; }
        public string trackingNumber { get; set; } 
        public string customerName { get; set; }
        public string customerPhone { get; set; }
        public string customerEmail { get; set; }
        public string customerAddress { get; set; } 
        public string adminNotes { get; set; }
        public decimal totalAmount { get; set; }
        public int statusId { get; set; }
        public List<DashboardOrderItemDTO> items { get; set; }
    }

    public class DashboardOrderItemDTO
    {
        public string productName { get; set; }
        public string size { get; set; }
        public int quantity { get; set; }
    }
}
