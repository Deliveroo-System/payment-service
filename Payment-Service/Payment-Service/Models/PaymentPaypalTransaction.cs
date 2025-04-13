using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payment_Service.Models
{
    [Table("payment_paypal_transactions")]
    public class PaymentPaypalTransaction
    {
        [Key]
        [Column("transaction_id")]
        public Guid TransactionId { get; set; }

        [Required]
        [Column("payment_id")]
        public Guid PaymentId { get; set; }

        [Required]
        [StringLength(255)]
        [Column("paypal_transaction_id")]
        public string PayPalTransactionId { get; set; }

        [Required]
        [StringLength(20)]
        [Column("transaction_status")]
        public string TransactionStatus { get; set; }

        [Required]
        [Column("transaction_amount")]
        public decimal TransactionAmount { get; set; }

        [Required]
        [StringLength(3)]
        [Column("transaction_currency")]
        public string TransactionCurrency { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("PaymentId")]
        public Payment Payment { get; set; }
    }
}
