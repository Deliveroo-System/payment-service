using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payment_Service.Models
{
    public class PaymentCODTransaction
    {
        [Key]
        public Guid CodId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid PaymentId { get; set; }

        [Required]
        [MaxLength(20)]
        public string CodStatus { get; set; } = "PENDING";

        [MaxLength(255)]
        public string? CollectedBy { get; set; } // Name of delivery agent

        public DateTime? CollectedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("PaymentId")]
        public Payment Payment { get; set; }
    }
}
