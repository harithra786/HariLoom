namespace hariloom.Models.DTOs
{
    public class OrderItemDTO
    {
        public int trnOrderItemId { get; set; }

        public int trnOrderId { get; set; }

        public int mstProductId { get; set; }

        public string productName { get; set; }

        public string coverImagePath { get; set; }

        public int quantity { get; set; }

        public string size { get; set; }

        public int price { get; set; }

        public bool isDelivered { get; set; }

        public DateTime? deliveredDate { get; set; }

        public DateTime orderDate { get; set; }

        public string orderNumber { get; set; }
    }

    public class UserOrdersDTO
    {
        public int trnOrderId { get; set; }

        public string orderNumber { get; set; }

        public DateTime orderDate { get; set; }

        public int totalAmount { get; set; }

        public int orderStatus { get; set; }

        public List<OrderItemDTO> orderItems { get; set; }
    }
}
