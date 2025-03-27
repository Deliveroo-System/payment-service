using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payment_Service.Models
{
    public class PaymentPaypalTransaction
    {
        [Key]
        public Guid TransactionId { get; set; }

        [Required]
        public Guid PaymentId { get; set; }

        [Required]
        [StringLength(255)]
        public string PayPalTransactionId { get; set; }

        [Required]
        [StringLength(20)]
        public string TransactionStatus { get; set; }

        [Required]
        public decimal TransactionAmount { get; set; }

        [Required]
        [StringLength(3)]
        public string TransactionCurrency { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("PaymentId")]
        public Payment Payment { get; set; }
    }
}
