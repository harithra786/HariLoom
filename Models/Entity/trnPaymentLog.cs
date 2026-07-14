using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hariloom.Models.Entity
{
    [Table("trnPaymentLog")]
    public class trnPaymentLog
    {
        [Key]
        public int trnPaymentLogId { get; set; }
        public int? trnOrderId { get; set; }
        public int? mstUserId { get; set; }
        public string? razorpayOrderId { get; set; }
        public string? razorpayPaymentId { get; set; }
        public string? razorpaySignature { get; set; }
        public string? paymentStatus { get; set; }
        public string? failureReason { get; set; }
        public decimal? amount { get; set; }
        public bool isActive { get; set; } = true;
        public DateTime createdDate { get; set; } = DateTime.Now;
    }
}