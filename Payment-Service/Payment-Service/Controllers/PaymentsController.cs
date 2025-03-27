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
                var paypalPayment = _payPalService.CreatePayment(payment.TotalAmount, payment.Currency,
                    "http://localhost:5212/api/payments/execute",
                    "http://localhost:5212/api/payments/cancel");

                return Ok(new { message = "PayPal payment initiated", paypalPayment });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing PayPal payment", error = ex.Message });
            }
        }

        // Pay with Cash on Delivery
        [HttpPost("pay/cod")]
        public IActionResult PayWithCOD([FromBody] LocalPayment payment)
        {
            try
            {
                // Hardcoded values for testing
                payment.OrderId = Guid.Parse("11111111-1111-1111-1111-111111111112");
                payment.UserId = Guid.Parse("22222222-2222-2222-2222-222222222221");
                payment.PaymentStatus = "PENDING";
                payment.CreatedAt = DateTime.UtcNow;
                payment.UpdatedAt = DateTime.UtcNow;

                _context.Payments.Add(payment);
                _context.SaveChanges();

                return Ok(new { message = "Cash on Delivery payment initiated", payment });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing Cash on Delivery payment", error = ex.Message });
            }
        }
    }
}
