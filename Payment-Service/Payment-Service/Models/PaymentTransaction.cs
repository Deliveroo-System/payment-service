using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payment_Service.Models
{
    public class PaymentTransaction
    {
        [Key]
        public Guid TransactionId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid PaymentId { get; set; }

        [MaxLength(255)]
        public string? GatewayTransactionId { get; set; } // Only for PayPal

        [Required]
        [MaxLength(20)]
        public string TransactionStatus { get; set; } = "PENDING";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("PaymentId")]
        public Payment Payment { get; set; }
    }
}
