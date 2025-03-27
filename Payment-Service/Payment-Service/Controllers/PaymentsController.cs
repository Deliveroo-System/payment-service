using Microsoft.AspNetCore.Mvc;
using Payment_Service.Models;
using PayPal.Api;
using Payment_Service.Controllers;
using Payment_Service.Service;

// Alias for Payment classes
using LocalPayment = Payment_Service.Models.Payment;
using PayPalPayment = PayPal.Api.Payment;

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
            _context.Payments.Add(payment);
            _context.SaveChanges();
            return Ok(payment);
        }

        // Pay with PayPal
        [HttpPost("pay/paypal")]
        public IActionResult PayWithPayPal([FromBody] LocalPayment payment)
        {
            var paypalPayment = _payPalService.CreatePayment(payment.TotalAmount, payment.Currency,
                "http://localhost:5212/api/payments/execute",
                "http://localhost:5212/api/payments/cancel");
            return Ok(paypalPayment);
        }

        // Pay with Cash on Delivery
        [HttpPost("pay/cod")]
        public IActionResult PayWithCOD([FromBody] LocalPayment payment)
        {
            payment.PaymentStatus = "PENDING";
            _context.Payments.Add(payment);
            _context.SaveChanges();
            return Ok(new { message = "Cash on Delivery payment initiated", payment });
        }
    }
}
