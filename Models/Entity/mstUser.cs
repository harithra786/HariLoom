using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hariloom.Models.Entity
{
    [Table("mstUser")]
    public class mstUser
    {
        [Key]
        public int mstUserId { get; set; }
        public string name { get; set; }
        public string phoneNumber { get; set; }
        public string password { get; set; }
        public string? email { get; set; }
        public string? address { get; set; }
        public string? addressLine1 { get; set; }
        public string? addressLine2 { get; set; }
        public string? city { get; set; }
        public string? state { get; set; }
        public string? zipCode { get; set; }
        public string? profileImageUrl { get; set; }
        public int accessLevel { get; set; } = 1;
        public DateTime createdDate { get; set; } = DateTime.Now;
        public bool isActive { get; set; } = true;
        public bool isPromotionalEmailOptIn { get; set; } = false;
        public string? ipAddress { get; set; }
        public int visitCount { get; set; } = 0;
    }

}