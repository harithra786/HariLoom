using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hariloom.Models.Entity
{
    [Table("trnOrderItems")]
    public class trnOrderItems
    {
        [Key]
        public int trnOrderItemsId { get; set; }
        public int trnOrderId { get; set; }
        [ForeignKey("trnOrderId")]
        [System.Text.Json.Serialization.JsonIgnore]
        public trnOrder order { get; set; }
        public int mstProductId { get; set; }
        [ForeignKey("mstProductId")]
        public mstProduct product { get; set; }
        public int quantity { get; set; }
        public string size { get; set; }
        public int price { get; set; }
        public bool isDelivered { get; set; } = false;
        public DateTime? deliveredDate { get; set; }
        public bool isActive { get; set; } = true;
        public int createdBy { get; set; }
        public DateTime createdDate { get; set; } = DateTime.Now;
        public int? updatedBy { get; set; }
        public DateTime? updatedDate { get; set; }
    }
}
