using hariloom.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hariloom.Models.Entity
{
    [Table("trnOrder")]
    public class trnOrder
    {
        [Key]
        public int trnOrderId { get; set; }
        public int mstUserId { get; set; }
        [ForeignKey("mstUserId")]
        public mstUser user { get; set; }
        public string orderNumber { get; set; }
        public DateTime orderDate { get; set; } = DateTime.Now;
        public int totalAmount { get; set; }
        public int orderStatus { get; set; } = (int)orderStatusEnum.Unpaid;
        public bool isActive { get; set; } = true;
        public int createdBy { get; set; }
        public DateTime createdDate { get; set; } = DateTime.Now;
        public int? updatedBy { get; set; }
        public DateTime? updatedDate { get; set; }
        public string? adminNotes { get; set; }
        public List<trnOrderItems> orderItems { get; set; } = new List<trnOrderItems>();
    }
}
