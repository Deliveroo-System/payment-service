using System;
using System.Collections.Generic;

namespace Payment_Service.Models;

public partial class Payment
{
    public int Id { get; set; }

    public string PaymentId { get; set; }

    public decimal Amount { get; set; }

    public string Currency { get; set; } 

    public string Status { get; set; } 

    public DateTime CreatedAt { get; set; }
}
