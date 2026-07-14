namespace hariloom.Models.DTOs
{
    public class UserAddressDTO
    {
        public int trnUserAddressId { get; set; }
        public int mstUserId { get; set; }
        public string label { get; set; }
        public string addressLine1 { get; set; }
        public string? addressLine2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zipCode { get; set; }
    }
}
