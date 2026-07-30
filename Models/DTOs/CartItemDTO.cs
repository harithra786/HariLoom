namespace hariloom.Models.DTOs
{
    public class ProductImageDTO
    {
        public string imageBase64 { get; set; }
        public string name { get; set; }
    }

    public class CartItemDTO
    {
        public int trnCartId { get; set; }
        public int mstProductId { get; set; }
        public string productName { get; set; }
        public string coverImageBase64 { get; set; }
        public string coverImagePath { get; set; }
        public List<ProductImageDTO> productImages { get; set; } = new List<ProductImageDTO>();
        public int quantity { get; set; }
        public string size { get; set; }
        public decimal unitPrice { get; set; }
        public decimal totalPrice { get; set; }
        public int basePrice { get; set; }
        public int deliveryCharge { get; set; }
        public int discountAmount { get; set; }
        public int quantityAvailable { get; set; }
    }
}