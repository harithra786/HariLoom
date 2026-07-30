using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hariloom.Models.Entity
{
    [Table("mstProduct")]
    public class mstProduct
    {
        [Key]
        public int mstProductId { get; set; }
        public string productName { get; set; }
        public string? productDisplayId { get; set; }
        public string? description { get; set; }
        public string? coverImagePath { get; set; }
        public int basePrice { get; set; }
        public int deliveryCharge { get; set; } = 0;
        public int discountAmount { get; set; } = 0;
        public int? discountedPrice { get; set; }
        public int quantityAvailable { get; set; }
        public string? productImages { get; set; }
        public string? washCareInstructions { get; set; }
        public string mstProductGroupingIds { get; set; } = string.Empty;
        public int mstProductMainCategoryId { get; set; }
        public int mstProductSubCategoryId { get; set; }
        public bool isAvailable { get; set; } = true;
        public bool isActive { get; set; } = true;
        public int createdBy { get; set; }
        public DateTime createdDate { get; set; } = DateTime.Now;
        public int? updatedBy { get; set; }
        public DateTime? updatedDate { get; set; }
        public int? deletedBy { get; set; }
        public DateTime? deletedDate { get; set; }
    }
}