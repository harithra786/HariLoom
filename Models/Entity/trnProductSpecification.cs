using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hariloom.Models.Entity
{
    [Table("trnProductSpecification")]
    public class trnProductSpecification
    {
        [Key]
        public int trnProductSpecificationId { get; set; }
        public int mstProductId { get; set; }
        public string title { get; set; }
        public string value { get; set; }

        [ForeignKey("mstProductId")]
        public mstProduct Product { get; set; }
        public bool isActive { get; set; } = true;
        public int createdBy { get; set; }
        public DateTime createdDate { get; set; } = DateTime.Now;
        public int? updatedBy { get; set; }
        public DateTime? updatedDate { get; set; }
        public int? deletedBy { get; set; }
        public DateTime? deletedDate { get; set; }
    }
}