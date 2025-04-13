using Microsoft.AspNetCore.Mvc;
using Payment_Service.Models;
using Payment_Service.Service;
using System;
using System.Linq;
using PayPal.Api;

// Aliases to avoid ambiguity
using LocalPayment = Payment_Service.Models.Payment;
using PayPalPayment = PayPal.Api.Payment;

namespace Payment_Service.Controllers
{
    [Route("api/payments")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly PaymentsDbContext _context;
        private readonly PayPalService _payPalService;

        // Constructor to inject dependencies (DbContext and PayPalService)
        public PaymentsController(PaymentsDbContext context, PayPalService payPalService)
        {
            _context = context;
            _payPalService = payPalService;
        }

        // Create a new local test payment
        [HttpPost("create")]
        public IActionResult CreatePayment([FromBody] LocalPayment payment)
        {
            try
            {
                // Validating the payment model before processing
                if (payment == null || payment.TotalAmount <= 0 || string.IsNullOrEmpty(payment.Currency))
                {
                    return BadRequest(new { message = "Invalid payment details." });
                }

                payment.OrderId = Guid.NewGuid(); // Generate a dummy OrderId
                payment.UserId = Guid.NewGuid();  // Generate a dummy UserId
                payment.PaymentStatus = "PENDING";
                payment.CreatedAt = DateTime.UtcNow;
                payment.UpdatedAt = DateTime.UtcNow;

                // Adding payment to the database
                _context.Payments.Add(payment);
                _context.SaveChanges();

                return Ok(new { message = "Payment created successfully", payment });
            }
            catch (Exception ex)
            {
                // Return detailed error messages
                return StatusCode(500, new
                {
                    message = "An error occurred while creating the payment",
                    error = ex.Message,
                    innerException = ex.InnerException?.Message
                });
            }
        }

        // Pay with PayPal (real sandbox)
        [HttpPost("pay/paypal")]
        public IActionResult PayWithPayPal([FromBody] LocalPayment payment)
        {
            try
            {
                // Validating the payment model
                if (payment == null || payment.TotalAmount <= 0 || string.IsNullOrEmpty(payment.Currency))
                {
                    return BadRequest(new { message = "Invalid payment details." });
                }

                // Step 1: Save the payment in DB
                payment.OrderId = Guid.NewGuid();
                payment.UserId = Guid.NewGuid();
                payment.PaymentStatus = "PENDING";
                payment.CreatedAt = DateTime.UtcNow;
                payment.UpdatedAt = DateTime.UtcNow;

                _context.Payments.Add(payment);
                _context.SaveChanges();

                // Step 2: Create PayPal Payment
                var createdPayment = _payPalService.CreatePayment(
                    payment.TotalAmount, payment.Currency,
                    "http://localhost:5212/api/payments/execute",  // return URL after PayPal payment approval
                    "http://localhost:5212/api/payments/cancel"    // cancel URL
                );

                // Step 3: Get redirect approval URL
                var approvalUrl = createdPayment.links
                    .FirstOrDefault(l => l.rel.Equals("approval_url", StringComparison.OrdinalIgnoreCase))?.href;

                // Optionally save PayPal transaction ID (not executed yet)
                var paypalTransaction = new PaymentPaypalTransaction
                {
                    PaymentId = payment.PaymentId,
                    PayPalTransactionId = createdPayment.id,
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
                    approval_url = approvalUrl,
                    paypalTransaction
                });
            }
            catch (Exception ex)
            {
                // Return error details
                return StatusCode(500, new
                {
                    message = "An error occurred while processing PayPal payment",
                    error = ex.Message,
                    innerException = ex.InnerException?.Message
                });
            }
        }

        // Cash On Delivery Payment
        [HttpPost("pay/cod")]
        public IActionResult PayWithCOD([FromBody] LocalPayment payment)
        {
            try
            {
                // Validating the payment model
                if (payment == null || payment.TotalAmount <= 0 || string.IsNullOrEmpty(payment.Currency))
                {
                    return BadRequest(new { message = "Invalid payment details." });
                }

                // Save the payment in DB
                payment.OrderId = Guid.NewGuid();
                payment.UserId = Guid.NewGuid();
                payment.PaymentStatus = "PENDING";
                payment.CreatedAt = DateTime.UtcNow;
                payment.UpdatedAt = DateTime.UtcNow;

                _context.Payments.Add(payment);
                _context.SaveChanges();

                // Create COD transaction entry
                var codTransaction = new PaymentCODTransaction
                {
                    PaymentId = payment.PaymentId,
                    CodStatus = "PENDING",
                    CreatedAt = DateTime.UtcNow
                };

                _context.PaymentCodTransactions.Add(codTransaction);
                _context.SaveChanges();

                return Ok(new { message = "Cash on Delivery payment initiated", payment, codTransaction });
            }
            catch (Exception ex)
            {
                // Return error details
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
