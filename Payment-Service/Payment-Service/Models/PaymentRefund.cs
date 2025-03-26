using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payment_Service.Models
{
    public class PaymentRefund
    {
        [Key]
        public Guid RefundId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid PaymentId { get; set; }

        [Required]
        public Guid TransactionId { get; set; } // Related transaction

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal RefundAmount { get; set; }

        [Required]
        [MaxLength(255)]
        public string RefundReason { get; set; }

        [Required]
        [MaxLength(20)]
        public string RefundStatus { get; set; } = "PENDING";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("PaymentId")]
        public Payment Payment { get; set; }

        [ForeignKey("TransactionId")]
        public PaymentTransaction Transaction { get; set; }
    }
}
