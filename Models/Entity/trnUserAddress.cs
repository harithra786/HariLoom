using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hariloom.Models.Entity
{
    [Table("trnUserAddress")]
    public class trnUserAddress
    {
        [Key]
        public int trnUserAddressId { get; set; }
        public int mstUserId { get; set; }
        public string label { get; set; }
        public string addressLine1 { get; set; }
        public string? addressLine2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zipCode { get; set; }
        public bool isActive { get; set; } = true;
        public DateTime createdDate { get; set; } = DateTime.Now;
        public DateTime? updatedDate { get; set; }
    }
}
