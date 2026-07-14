using System.ComponentModel.DataAnnotations;

namespace hariloom.Models.Entity
{
    public class mstFAQ
    {
        [Key]
        public int mstFAQId { get; set; }
        public string question { get; set; }
        public string answer { get; set; }
        public bool isActive { get; set; }
        public int createdBy { get; set; }
        public DateTime createdDate { get; set; }
        public int? updatedBy { get; set; }
        public DateTime? updatedDate { get; set; }
        public int? deletedBy { get; set; }
        public DateTime? deletedDate { get; set; }
    }
}
