using Payment_Service.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payment_Service.Models
{
    public class PaymentLog
    {
        [Key]
        public Guid LogId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid PaymentId { get; set; }

        [Required]
        [MaxLength(50)]
        public string ActionType { get; set; }

        [Required]
        public string LogMessage { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("PaymentId")]
        public Payment Payment { get; set; }
    }
}
