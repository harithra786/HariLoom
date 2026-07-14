namespace hariloom.Models.DTOs
{
    public class UpdateTrackingDTO
    {
        public long OrderId { get; set; }
        public string? TrackingId { get; set; }
        public string? Status { get; set; }
        public long? UpdatedBy { get; set; }
    }
}
