using Microsoft.AspNetCore.Mvc;
using Payment_Service.Models;
using Payment_Service.Service;
using System;
using System.Linq;
using System.Threading.Tasks;
using PayPalCheckoutSdk.Orders;

namespace Payment_Service.Controllers
{
    [Route("api/payments")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly PaymentsDbContext _context;
        private readonly PayPalService _payPalService;

        public PaymentsController(PaymentsDbContext context, PayPalService payPalService)
        {
            _context = context;
            _payPalService = payPalService;
        }

        // ✅ Local test payment creation
        [HttpPost("create")]
        public IActionResult CreatePayment([FromBody] Payment payment)
        {
            try
            {
                if (payment == null || payment.TotalAmount <= 0 || string.IsNullOrEmpty(payment.Currency))
                    return BadRequest(new { message = "Invalid payment details." });

                payment.OrderId = Guid.NewGuid();
                payment.UserId = Guid.NewGuid();
                payment.PaymentStatus = "PENDING";
                payment.CreatedAt = DateTime.UtcNow;
                payment.UpdatedAt = DateTime.UtcNow;

                _context.Payments.Add(payment);
                _context.SaveChanges();

                return Ok(new { message = "Payment created successfully", payment });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while creating the payment",
                    error = ex.Message,
                    innerException = ex.InnerException?.Message
                });
            }
        }

        // ✅ PayPal Payment (Real)
      [HttpPost("pay/paypal")]
public async Task<IActionResult> PayWithPayPal([FromBody] Payment payment)
{
    try
    {
        if (payment == null || payment.TotalAmount <= 0 || string.IsNullOrEmpty(payment.Currency))
            return BadRequest(new { message = "Invalid payment details." });

        payment.OrderId = Guid.NewGuid();
        payment.UserId = Guid.NewGuid();
        payment.PaymentStatus = "PENDING";
        payment.PaymentMethod = "PayPal";
        payment.CreatedAt = DateTime.UtcNow;
        payment.UpdatedAt = DateTime.UtcNow;

        // Save the payment details into the database synchronously
        _context.Payments.Add(payment);
        _context.SaveChanges();

        // Now, call the PayPal service asynchronously
        var createdOrder = await _payPalService.CreatePayment(
            payment.TotalAmount,
            payment.Currency,
            "http://localhost:5212/api/payments/execute",  // Return URL
            "http://localhost:5212/api/payments/cancel"    // Cancel URL
        );

        var approvalLink = createdOrder.Links?
            .FirstOrDefault(link => link.Rel.Equals("approve", StringComparison.OrdinalIgnoreCase))?.Href;

        // Save PayPal transaction
        var paypalTransaction = new PaymentPaypalTransaction
        {
            PaymentId = payment.PaymentId,
            PayPalTransactionId = createdOrder.Id,
            TransactionStatus = "PENDING",
            TransactionAmount = payment.TotalAmount,
            TransactionCurrency = payment.Currency,
            CreatedAt = DateTime.UtcNow
        };

        _context.PaymentPaypalTransactions.Add(paypalTransaction);
        _context.SaveChanges();

        return Ok(new
        {
            message = "Redirect the user to PayPal to approve the payment",
            approval_url = approvalLink,
            paypalTransaction
        });
    }
    catch (Exception ex)
    {
        return StatusCode(500, new
        {
            message = "An error occurred while processing PayPal payment",
            error = ex.Message,
            innerException = ex.InnerException?.Message
        });
    }
}

        // ✅ Cash on Delivery Payment
        [HttpPost("pay/cod")]
        public IActionResult PayWithCOD([FromBody] Payment payment)
        {
            try
            {
                // Validate payment details
                if (payment == null || payment.TotalAmount <= 0 || string.IsNullOrEmpty(payment.Currency))
                    return BadRequest(new { message = "Invalid payment details." });

                // Initialize properties
                payment.OrderId = Guid.NewGuid();
                payment.UserId = Guid.NewGuid();
                payment.PaymentStatus = "PENDING";
                payment.PaymentMethod = "CashOnDelivery";
                payment.CreatedAt = DateTime.UtcNow;
                payment.UpdatedAt = DateTime.UtcNow;

                // Add payment to the database
                _context.Payments.Add(payment);
                _context.SaveChanges();

                // Save COD transaction details in the database
                var codTransaction = new PaymentCODTransaction
                {
                    PaymentId = payment.PaymentId,
                    CodStatus = "PENDING",
                    CreatedAt = DateTime.UtcNow
                };

                _context.PaymentCodTransactions.Add(codTransaction);
                _context.SaveChanges();

                return Ok(new
                {
                    message = "Cash on Delivery payment initiated",
                    payment,
                    codTransaction
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while processing Cash on Delivery payment",
                    error = ex.Message,
                    innerException = ex.InnerException?.Message
                });
            }
        }
    }
}
