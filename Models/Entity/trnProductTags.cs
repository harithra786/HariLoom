using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hariloom.Models.Entity
{
    [Table("trnProductTags")]
    public class trnProductTags
    {
        [Key]
        public int trnProductTagsId { get; set; }
        public int mstProductId { get; set; }
        public string tag { get; set; }
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