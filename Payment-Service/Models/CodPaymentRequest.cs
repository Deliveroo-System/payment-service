using System.ComponentModel.DataAnnotations;

namespace Payment_Service.Models
{
    public class CodPaymentRequest
    {
        [Required]
        public Guid OrderId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        [Required]
        public string Currency { get; set; } = "USD";
    }
}
