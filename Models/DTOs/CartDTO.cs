namespace hariloom.Models.DTOs
{
    public class AddToCartDTO
    {
        public int productId { get; set; }
        public string size { get; set; }
        public int quantity { get; set; }
        public int userId { get; set; }
    }

    public class UpdateCartQuantityDTO
    {
        public int cartId { get; set; }
        public int quantity { get; set; }
        public int userId { get; set; }
    }

    public class RemoveFromCartDTO
    {
        public int productId { get; set; }
        public int userId { get; set; }
    }

    public class CreateOrderDTO
    {
        public int productId { get; set; }
        public string size { get; set; }
        public int quantity { get; set; }
        public int userId { get; set; }
    }

    public class VerifyPaymentDTO
    {
        public int orderId { get; set; }
        public string razorpayOrderId { get; set; }
        public string razorpayPaymentId { get; set; }
        public string razorpaySignature { get; set; }
        public bool isCartCheckout { get; set; }
    }
}
