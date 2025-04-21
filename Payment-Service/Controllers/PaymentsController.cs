using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Payment_Service.Models;
using Payment_Service.Service;
using PayPalCheckoutSdk.Orders;
using System;
using System.Linq;
using System.Threading.Tasks;
using LocalPayment = Payment_Service.Models.Payment;

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

        // 🔹 Helper to prepare initial payment
        private LocalPayment CreateBasePayment(LocalPayment payment)
        {
            payment.OrderId = Guid.NewGuid(); // You could replace this with actual OrderId if it comes from another service
            payment.UserId = Guid.NewGuid();  // Replace with real authenticated user ID in real app
            payment.PaymentStatus = "PENDING";
            payment.CreatedAt = DateTime.UtcNow;
            payment.UpdatedAt = DateTime.UtcNow;
            return payment;
        }

        // ✅ Create a generic payment entry (not used for PayPal transactions)
        [HttpPost("create")]
        public IActionResult CreatePayment([FromBody] LocalPayment payment)
        {
            try
            {
                var newPayment = CreateBasePayment(payment);
                _context.Payments.Add(newPayment);
                _context.SaveChanges();

                return Ok(new { message = "Payment created successfully", payment = newPayment });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating payment", error = ex.Message });
            }
        }

        // ✅ PayPal: Create payment
        [HttpPost("pay/paypal")]
        public async Task<IActionResult> PayWithPayPal([FromBody] LocalPayment payment)
        {
            try
            {
                var newPayment = CreateBasePayment(payment);
                _context.Payments.Add(newPayment);
                await _context.SaveChangesAsync();

                var paypalOrder = await _payPalService.CreatePayment(
                    newPayment.TotalAmount,
                    newPayment.Currency,
                    "https://localhost:1512/api/payments/success", // 🔄 Replace with actual deployed URL
                    "https://localhost:1512/api/payments/cancel"   // 🔄 Replace with actual deployed URL
                );

                var approvalUrl = paypalOrder.Links.FirstOrDefault(link => link.Rel == "approve")?.Href;

                if (string.IsNullOrEmpty(approvalUrl))
                {
                    throw new Exception("No approval URL found in PayPal response.");
                }

                var paypalTransaction = new PaymentPaypalTransaction
                {
                    PaymentId = newPayment.PaymentId,
                    PayPalTransactionId = paypalOrder.Id,
                    TransactionStatus = "CREATED",
                    TransactionAmount = newPayment.TotalAmount,
                    TransactionCurrency = newPayment.Currency,
                    CreatedAt = DateTime.UtcNow
                };

                _context.PaymentPaypalTransactions.Add(paypalTransaction);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "PayPal payment created. Redirect user to approval URL.",
                    approvalUrl,
                    paymentId = newPayment.PaymentId,
                    paypalOrderId = paypalOrder.Id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "PayPal payment failed",
                    error = ex.Message,
                    inner = ex.InnerException?.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        // ✅ PayPal: Capture payment (after user approves)
        [HttpGet("success")]
        public async Task<IActionResult> ExecutePayment([FromQuery] string token)
        {
            try
            {
                var result = await _payPalService.CapturePayment(token);
                var orderId = result.Id;

                var transaction = await _context.PaymentPaypalTransactions
                    .FirstOrDefaultAsync(t => t.PayPalTransactionId == orderId);

                if (transaction == null)
                    return BadRequest(new { message = "Transaction not found" });

                transaction.TransactionStatus = "COMPLETED";

                var payment = await _context.Payments
                    .FirstOrDefaultAsync(p => p.PaymentId == transaction.PaymentId);

                if (payment != null)
                {
                    payment.PaymentStatus = "COMPLETED";
                    payment.UpdatedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                // Redirect to your frontend success page
                return Redirect("https://yourfrontend.com/payment-success");
            }
            catch (Exception ex)
            {
                return Redirect($"https://yourfrontend.com/payment-error?message={Uri.EscapeDataString(ex.Message)}");
            }
        }

        // ✅ PayPal: Cancel
        [HttpGet("cancel")]
        public IActionResult CancelPayment()
        {
            return Redirect("https://yourfrontend.com/payment-cancelled");
        }

        // ✅ COD (Cash on Delivery)
        [HttpPost("pay/cod")]
        public IActionResult PayWithCOD([FromBody] LocalPayment payment)
        {
            try
            {
                var newPayment = CreateBasePayment(payment);
                _context.Payments.Add(newPayment);
                _context.SaveChanges();

                var codTransaction = new PaymentCODTransaction
                {
                    PaymentId = newPayment.PaymentId,
                    CodStatus = "PENDING",
                    CreatedAt = DateTime.UtcNow
                };

                _context.PaymentCodTransactions.Add(codTransaction);
                _context.SaveChanges();

                return Ok(new
                {
                    message = "Cash on Delivery payment initiated",
                    payment = newPayment,
                    codTransaction
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "COD payment failed",
                    error = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
        }
    }
}
