using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hariloom.Models.Entity
{
    [Table("trnProductSize")]
    public class trnProductSize
    {
        [Key]
        public int trnProductSizeId { get; set; }
        public int mstProductId { get; set; }
        public int trnProductColorId { get; set; }
        public string size { get; set; }
        public int quantityAvailable { get; set; }
        public bool isActive { get; set; } = true;
        public int createdBy { get; set; }
        public DateTime createdDate { get; set; } = DateTime.Now;
        public int? updatedBy { get; set; }
        public DateTime? updatedDate { get; set; }
        public int? deletedBy { get; set; }
        public DateTime? deletedDate { get; set; }

        [ForeignKey("mstProductId")]
        public mstProduct Product { get; set; }
    }

}