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

        // ✅ Create a generic payment (not linked to any gateway)
        [HttpPost("create")]
        public IActionResult CreatePayment([FromBody] LocalPayment payment)
        {
            try
            {
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
                return StatusCode(500, new { message = "Error while creating payment", error = ex.Message });
            }
        }

        // ✅ Pay with PayPal
        [HttpPost("pay/paypal")]
        public async Task<IActionResult> PayWithPayPal([FromBody] LocalPayment payment)
        {
            try
            {
                payment.OrderId = Guid.NewGuid();
                payment.UserId = Guid.NewGuid();
                payment.PaymentStatus = "PENDING";
                payment.CreatedAt = DateTime.UtcNow;
                payment.UpdatedAt = DateTime.UtcNow;

                _context.Payments.Add(payment);
                _context.SaveChanges();

                var paypalOrder = await _payPalService.CreatePayment(payment.TotalAmount, payment.Currency,
                    "http://localhost:5212/api/payments/execute",
                    "http://localhost:5212/api/payments/cancel");

                var approvalLink = paypalOrder.Links.FirstOrDefault(link => link.Rel == "approve")?.Href;

                var paypalTransaction = new PaymentPaypalTransaction
                {
                    PaymentId = payment.PaymentId,
                    PayPalTransactionId = paypalOrder.Id,
                    TransactionStatus = "CREATED",
                    TransactionAmount = payment.TotalAmount,
                    TransactionCurrency = payment.Currency,
                    CreatedAt = DateTime.UtcNow
                };

                _context.PaymentPaypalTransactions.Add(paypalTransaction);
                _context.SaveChanges();

                return Ok(new
                {
                    message = "PayPal payment created. Redirect the user to approval URL.",
                    approvalUrl = approvalLink,
                    payment,
                    paypalTransaction
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error while processing PayPal payment",
                    error = ex.Message,
                    innerException = ex.InnerException?.Message
                });
            }
        }

        // ✅ Pay with Cash on Delivery
        [HttpPost("pay/cod")]
        public IActionResult PayWithCOD([FromBody] LocalPayment payment)
        {
            Console.WriteLine("🚀 PayWithPayPal endpoint hit");
            try
            {
                payment.OrderId = Guid.NewGuid();
                payment.UserId = Guid.NewGuid();
                payment.PaymentStatus = "PENDING";
                payment.CreatedAt = DateTime.UtcNow;
                payment.UpdatedAt = DateTime.UtcNow;

                _context.Payments.Add(payment);
                _context.SaveChanges();

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
                    message = "Error while processing COD payment",
                    error = ex.Message,
                    innerException = ex.InnerException?.Message
                });
            }

           
        }

        [HttpGet("execute")]
        public async Task<IActionResult> ExecutePayment([FromQuery] string token)
        {
            try
            {
                var request = new OrdersCaptureRequest(token);
                request.RequestBody(new OrderActionRequest());

                var response = await PayPalClient.Client().Execute(request);
                var result = response.Result<Order>();

                // TODO: Update payment status in your DB (e.g., mark it as COMPLETED)

                return Ok(new
                {
                    message = "Payment successful!",
                    details = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Payment execution failed", error = ex.Message });
            }
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("Pong!");
        }
    }
}
