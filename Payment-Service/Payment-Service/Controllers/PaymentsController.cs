using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Payment_Service.Models;
using Payment_Service.Service;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Payment_Service.Controllers
{
    [Route("api/payment")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentsDbContext _context;
        private readonly PayPalService _payPalService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(PaymentsDbContext context, PayPalService payPalService, ILogger<PaymentController> logger)
        {
            _context = context;
            _payPalService = payPalService;
            _logger = logger;
        }

        [HttpPost("create-paypal")]
        public async Task<IActionResult> CreatePayPalPayment([FromBody] Payment payment)
        {
            if (payment.PaymentMethod != "PAYPAL")
                return BadRequest("Invalid payment method.");

            if (payment.TotalAmount <= 0)
                return BadRequest("Total amount must be greater than zero.");

            if (payment.UserId == Guid.Empty)
            {
                payment.UserId = Guid.NewGuid();  // Generate a new GUID if not provided
                _logger.LogInformation($"Generated new userId: {payment.UserId}");
            }

            try
            {
                var payPalOrderId = await _payPalService.CreatePayPalOrder(payment.TotalAmount, payment.Currency);
                _logger.LogInformation($"PayPal Order Created: {payPalOrderId}");

                payment.PaymentStatus = "PENDING";
                payment.OrderId = payPalOrderId;
                _context.Payment.Add(payment);
                await _context.SaveChangesAsync();

                return Ok(new { PaymentId = payment.PaymentId, PayPalOrderId = payPalOrderId, UserId = payment.UserId });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating PayPal order: {ex.Message}", ex);
                return StatusCode(500, "Error creating PayPal order.");
            }
        }





        [HttpPost("capture-paypal/{paymentId}")]
        public async Task<IActionResult> CapturePayPalPayment(Guid paymentId, [FromQuery] string orderId)
        {
            if (string.IsNullOrWhiteSpace(orderId))
                return BadRequest("Invalid PayPal Order ID.");

            orderId = orderId.Trim(); // Ensure no newline or extra spaces

            var payment = await _context.Payment.FindAsync(paymentId);
            if (payment == null)
                return NotFound("Payment not found.");

            try
            {
                var status = await _payPalService.CapturePayPalPayment(orderId);
                _logger.LogInformation($"PayPal Capture Response: OrderID {orderId}, Status {status}");

                payment.PaymentStatus = status == "COMPLETED" ? "COMPLETED" : "FAILED";

                // Add payment log entry
                _context.PaymentLogs.Add(new PaymentLog
                {
                    PaymentId = paymentId,
                    ActionType = status == "COMPLETED" ? "PAYMENT_COMPLETED" : "PAYMENT_FAILED",
                    LogMessage = $"PayPal transaction status: {status}"
                });

                await _context.SaveChangesAsync();
                return Ok(new { PaymentId = paymentId, Status = status });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error capturing PayPal payment: {ex.Message}");
                return StatusCode(500, "Error capturing PayPal payment.");
            }
        }
    }
}
