using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Payment_Service.Models
{
    [Table("payment")]
    public class Payment
    {
        [Key]
        [Column("payment_id")]
        public Guid PaymentId { get; set; } = Guid.NewGuid();

        [Column("order_id")]
        public Guid OrderId { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("total_amount")]
        public decimal TotalAmount { get; set; }

        [Column("currency")]
        public string Currency { get; set; } = "USD";

        [Required]
        [Column("payment_method")]
        public string PaymentMethod { get; set; } = string.Empty;

        [Column("payment_status")]
        public string PaymentStatus { get; set; } = "PENDING";

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Use InverseProperty to guide EF
        [InverseProperty(nameof(PaymentPaypalTransaction.Payment))]
        public ICollection<PaymentPaypalTransaction>? PaypalTransactions { get; set; }

        public PaymentCODTransaction? CodTransaction { get; set; }
    }

}
