using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hariloom.Models.Entity
{
    [Table("mstProductSubCategory")]
    public class mstProductSubCategory
    {
        [Key]
        public int mstProductSubCategoryId { get; set; }
        
        [ForeignKey("MainCategory")]
        public int mstProductMainCategoryId { get; set; }
        
        public string subCategoryName { get; set; } = string.Empty;
        public string? subCategoryImagePath { get; set; }
        public bool isActive { get; set; } = true;
        public int createdBy { get; set; }
        public DateTime createdDate { get; set; } = DateTime.Now;
        public int? updatedBy { get; set; }
        public DateTime? updatedDate { get; set; }

        public virtual mstProductMainCategory MainCategory { get; set; } = null!;
    }
}
