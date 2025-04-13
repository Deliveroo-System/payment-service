using Microsoft.AspNetCore.Mvc;
using Payment_Service.Models;
using Payment_Service.Service;
using System;

// Alias for Payment classes
using LocalPayment = Payment_Service.Models.Payment;


namespace Payment_Service.Controllers
{
    [Route("api/payments")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentsDbContext _context;
        private readonly PayPalService _payPalService;

        public PaymentController(PaymentsDbContext context, PayPalService payPalService)
        {
            _context = context;
            _payPalService = payPalService;
        }

        // Create a new payment
        [HttpPost("create")]
        public IActionResult CreatePayment([FromBody] LocalPayment payment)
        {
            try
            {
                // Hardcoded values for testing
                payment.OrderId = Guid.Parse("11111111-1111-1111-1111-111111111111");
                payment.UserId = Guid.Parse("22222222-2222-2222-2222-222222222222");
                payment.PaymentStatus = "PENDING";
                payment.CreatedAt = DateTime.UtcNow;
                payment.UpdatedAt = DateTime.UtcNow;

                _context.Payments.Add(payment);
                _context.SaveChanges();

                return Ok(new { message = "Payment created successfully", payment });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the payment", error = ex.Message });
            }
        }

        // Pay with PayPal
        [HttpPost("pay/paypal")]
        public IActionResult PayWithPayPal([FromBody] LocalPayment payment)
        {
            try
            {
                // Step 1: Save Payment to DB
                payment.OrderId = Guid.Parse("11111111-1111-1111-1111-111111111122");
                payment.UserId = Guid.Parse("22222222-2222-2222-2222-222222222222");
                payment.PaymentStatus = "PENDING";
                payment.CreatedAt = DateTime.UtcNow;
                payment.UpdatedAt = DateTime.UtcNow;

                _context.Payments.Add(payment);
                _context.SaveChanges();  // Save to get PaymentId

                // Step 2: Call PayPal service to create payment (fake or real)
                var paypalPayment = _payPalService.CreatePayment(payment.TotalAmount, payment.Currency,
                    "http://localhost:5212/api/payments/execute",
                    "http://localhost:5212/api/payments/cancel");

                // Step 3: Save PayPal Transaction
                var paypalTransaction = new PaymentPaypalTransaction
                {
                    PaymentId = payment.PaymentId,
                    PayPalTransactionId = paypalPayment.id,
                    TransactionStatus = "SUCCESS",  // ✅ Use allowed value
                    TransactionAmount = payment.TotalAmount,
                    TransactionCurrency = payment.Currency,
                    CreatedAt = DateTime.UtcNow
                };
                _context.PaymentPaypalTransactions.Add(paypalTransaction);
                _context.SaveChanges();

                return Ok(new
                {
                    message = "PayPal payment initiated",
                    payment,
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


        [HttpPost("pay/cod")]
        public IActionResult PayWithCOD([FromBody] LocalPayment payment)
        {
            try
            {
                // Create and save the payment
                payment.OrderId = Guid.Parse("11111111-1111-1111-1111-111111111116");
                payment.UserId = Guid.Parse("22222222-2222-2222-2222-222222222226");
                payment.PaymentStatus = "PENDING";
                payment.CreatedAt = DateTime.UtcNow;
                payment.UpdatedAt = DateTime.UtcNow;

                _context.Payments.Add(payment);
                _context.SaveChanges();  // ✅ Save payment first to get PaymentId

                // Create and save the COD transaction
                var codTransaction = new PaymentCODTransaction
                {
                    PaymentId = payment.PaymentId,  // ✅ Ensure this exists
                    CodStatus = "PENDING",
                    CreatedAt = DateTime.UtcNow
                };

                _context.PaymentCodTransactions.Add(codTransaction);
                _context.SaveChanges();

                return Ok(new { message = "Cash on Delivery payment initiated", payment, codTransaction });
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
