using Microsoft.AspNetCore.Mvc;
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

        // ✅ Create Local Payment (used for COD and PayPal)
        private LocalPayment CreateBasePayment(LocalPayment payment)
        {
            payment.OrderId = Guid.NewGuid();
            payment.UserId = Guid.NewGuid();
            payment.PaymentStatus = "PENDING";
            payment.CreatedAt = DateTime.UtcNow;
            payment.UpdatedAt = DateTime.UtcNow;
            return payment;
        }

        // ✅ Generic Payment Creation (Optional)
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

        // ✅ PayPal Payment
        [HttpPost("pay/paypal")]
        public async Task<IActionResult> PayWithPayPal([FromBody] LocalPayment payment)
        {
            try
            {
                var newPayment = CreateBasePayment(payment);
                _context.Payments.Add(newPayment);
                _context.SaveChanges();

                var paypalOrder = await _payPalService.CreatePayment(
                    newPayment.TotalAmount,
                    newPayment.Currency,
                    "http://localhost:5212/api/payments/execute",
                    "http://localhost:5212/api/payments/cancel"
                );

                var approvalUrl = paypalOrder.Links.FirstOrDefault(link => link.Rel == "approve")?.Href;

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
                _context.SaveChanges();

                return Ok(new
                {
                    message = "PayPal payment created. Redirect user to approval URL.",
                    approvalUrl,
                    payment = newPayment,
                    paypalTransaction
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "PayPal payment failed", error = ex.Message, inner = ex.InnerException?.Message });
            }
        }

        // ✅ COD Payment
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
                return StatusCode(500, new { message = "COD payment failed", error = ex.Message, inner = ex.InnerException?.Message });
            }
        }

        // ✅ Execute PayPal Payment
        [HttpGet("execute")]
        public async Task<IActionResult> ExecutePayment([FromQuery] string token)
        {
            try
            {
                var result = await _payPalService.CapturePayment(token);
                var transactionId = result.Id;

                var transaction = _context.PaymentPaypalTransactions.FirstOrDefault(t => t.PayPalTransactionId == transactionId);
                if (transaction != null)
                {
                    transaction.TransactionStatus = "COMPLETED";
                    var payment = _context.Payments.FirstOrDefault(p => p.PaymentId == transaction.PaymentId);
                    if (payment != null)
                    {
                        payment.PaymentStatus = "COMPLETED";
                        payment.UpdatedAt = DateTime.UtcNow;
                    }

                    await _context.SaveChangesAsync();
                }

                return Ok(new { message = "PayPal payment executed successfully", result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Payment execution failed", error = ex.Message });
            }
        }

        // ✅ Ping
        [HttpGet("ping")]
        public IActionResult Ping() => Ok("Pong!");
    }
}
