using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hariloom.Models.Entity
{
    [Table("mstProductMainCategory")]
    public class mstProductMainCategory
    {
        [Key]
        public int mstProductMainCategoryId { get; set; }
        public string mainCategoryName { get; set; } = string.Empty;
        public string? mainCategoryImagePath { get; set; }
        public bool isActive { get; set; } = true;
        public int createdBy { get; set; }
        public DateTime createdDate { get; set; } = DateTime.Now;
        public int? updatedBy { get; set; }
        public DateTime? updatedDate { get; set; }
    }
}
