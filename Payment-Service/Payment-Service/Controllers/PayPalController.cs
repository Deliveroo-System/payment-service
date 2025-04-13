//using Microsoft.AspNetCore.Mvc;
//using Payment_Service.Models;
//using Payment_Service.Service;
//using System;
//using System.Linq;

//using LocalPayment = Payment_Service.Models.Payment;

//namespace Payment_Service.Controllers
//{
//    [Route("api/payments")]
//    [ApiController]
//    public class PaymentController : ControllerBase
//    {
//        private readonly PaymentsDbContext _context;
//        private readonly PayPalService _payPalService;

//        public PaymentController(PaymentsDbContext context, PayPalService payPalService)
//        {
//            _context = context;
//            _payPalService = payPalService;
//        }

//        [HttpPost("pay/paypal")]
//        public IActionResult PayWithPayPal([FromBody] LocalPayment payment)
//        {
//            try
//            {
//                // Dummy order and user IDs for testing
//                payment.OrderId = Guid.NewGuid();
//                payment.UserId = Guid.NewGuid();
//                payment.PaymentStatus = "PENDING";
//                payment.CreatedAt = DateTime.UtcNow;
//                payment.UpdatedAt = DateTime.UtcNow;

//                _context.Payments.Add(payment);
//                _context.SaveChanges();

//                var createdPayment = _payPalService.CreatePayment(
//                    payment.TotalAmount, payment.Currency,
//                    "http://localhost:5212/api/payments/execute",
//                    "http://localhost:5212/api/payments/cancel"
//                );

//                var approvalUrl = createdPayment.links
//                    .FirstOrDefault(l => l.rel.Equals("approval_url", StringComparison.OrdinalIgnoreCase))?.href;

//                return Ok(new
//                {
//                    message = "Redirect the user to PayPal to approve the payment",
//                    approval_url = approvalUrl,
//                    paymentId = createdPayment.id
//                });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new
//                {
//                    message = "Error initiating PayPal payment",
//                    error = ex.Message,
//                    innerException = ex.InnerException?.Message
//                });
//            }
//        }

//        [HttpGet("execute")]
//        public IActionResult ExecutePayment([FromQuery] string paymentId, [FromQuery] string payerId)
//        {
//            try
//            {
//                var executedPayment = _payPalService.ExecutePayment(paymentId, payerId);

//                // You can optionally update your database here if needed

//                return Ok(new
//                {
//                    message = "Payment executed successfully",
//                    executedPayment
//                });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new
//                {
//                    message = "Error executing PayPal payment",
//                    error = ex.Message,
//                    innerException = ex.InnerException?.Message
//                });
//            }
//        }

//        [HttpGet("cancel")]
//        public IActionResult CancelPayment()
//        {
//            return Ok(new { message = "Payment cancelled by the user." });
//        }
//    }
//}
