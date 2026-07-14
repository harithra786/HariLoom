namespace hariloom.Models.DTOs
{
    public class FaqDTO
    {
        public int mstFAQId { get; set; }
        public string question { get; set; }
        public string answer { get; set; }
        public bool isActive { get; set; }
        public DateTime createdDate { get; set; }
    }

    public class AddFaqDTO
    {
        public string question { get; set; }
        public string answer { get; set; }
        public int userId { get; set; }
    }

    public class UpdateFaqDTO
    {
        public int mstFAQId { get; set; }
        public string question { get; set; }
        public string answer { get; set; }
        public int userId { get; set; }
    }
}
