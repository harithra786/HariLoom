namespace hariloom.Models.DTOs
{
    public class WishlistDTO
    {
        public int trnWishlistId { get; set; }
        public int mstUserId { get; set; }
        public int mstProductId { get; set; }
        public string productName { get; set; }
        public string coverImageBase64 { get; set; }
        public string coverImagePath { get; set; }
        public decimal basePrice { get; set; }
        public decimal? discountedPrice { get; set; }
        public DateTime addedDate { get; set; }
        public bool isActive { get; set; }
    }

    public class AddToWishlistDTO
    {
        public int mstUserId { get; set; }
        public int mstProductId { get; set; }
    }

    public class WishlistCheckDTO
    {
        public int mstUserId { get; set; }
        public List<int> productIds { get; set; } = new List<int>();
    }

    public class WishlistStatusDTO
    {
        public int mstProductId { get; set; }
        public bool isWishlisted { get; set; }
    }
}
