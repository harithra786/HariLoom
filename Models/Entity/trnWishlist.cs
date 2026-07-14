using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hariloom.Models.Entity
{
    [Table("trnWishlist")]
    public class trnWishlist
    {
        [Key]
        public int trnWishlistId { get; set; }

        public int mstUserId { get; set; }

        [ForeignKey("mstUserId")]
        public mstUser user { get; set; }

        public int mstProductId { get; set; }

        [ForeignKey("mstProductId")]
        public mstProduct product { get; set; }

        public bool isActive { get; set; } = true;
        public int createdBy { get; set; }
        public DateTime createdDate { get; set; } = DateTime.Now;
        public int? updatedBy { get; set; }
        public DateTime? updatedDate { get; set; }
        public int? deletedBy { get; set; }
        public DateTime? deletedDate { get; set; }
    }
}

