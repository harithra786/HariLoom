using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hariloom.Models.Entity
{
    [Table("trnProductColor")]
    public class trnProductColor
    {
        [Key]
        public int trnProductColorId { get; set; }
        public int mstProductId { get; set; }
        public string color { get; set; }
        public string colorHex { get; set; }
        public string colorImagePath { get; set; }
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
