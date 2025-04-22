namespace Payment_Service.Models
{
    public class PaypalPaymentRequest
    {
        
            public Guid OrderId { get; set; }       // ID from your system
            public Guid UserId { get; set; }        // ID of the user paying
            public decimal Amount { get; set; }     // How much to pay
            public string Currency { get; set; } = "USD"; // Default to USD
        
    }
}

