using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payment_Service.Models
{
    [Table("payment_cod_transactions")]
    public class PaymentCODTransaction
    {
        [Key]
        [Column("cod_id")]
        public Guid CodId { get; set; } = Guid.NewGuid();

        [Required]
        [Column("payment_id")]
        public Guid PaymentId { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("cod_status")]
        public string CodStatus { get; set; } = "PENDING";

        [MaxLength(255)]
        [Column("collected_by")]
        public string? CollectedBy { get; set; }

        [Column("collected_at")]
        public DateTime? CollectedAt { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ✅ Explicitly define the foreign key to avoid "payment_id1" issue
        [ForeignKey(nameof(PaymentId))]
        public Payment Payment { get; set; }
    }
}
