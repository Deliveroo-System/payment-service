using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Payment_Service.Models;
using Payment_Service.Service;

namespace Payment_Service.Controllers
{
    [Route("api/payments")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly PayPalService _payPalService;
        private readonly PaymentsDbContext _dbContext; // Injected DbContext

        public PaymentsController(PayPalService payPalService, PaymentsDbContext dbContext)
        {
            _payPalService = payPalService;
            _dbContext = dbContext;
        }

        [HttpPost("create")]
        public IActionResult CreatePayment([FromBody] PaymentRequest paymentRequest)
        {
            try
            {
                if (paymentRequest.Amount <= 0)
                {
                    return BadRequest(new { message = "Invalid payment amount." });
                }

                // Pass the decimal Amount to the PayPal service, it will be converted to a string
                var payment = _payPalService.CreatePayment(paymentRequest.Amount, "USD", "https://yourfrontend.com/success", "https://yourfrontend.com/cancel");

                // Save payment details to the database
                var paymentRecord = new Payment
                {
                    PaymentId = payment.id,
                    Amount = paymentRequest.Amount,
                    Currency = "USD",  // Assuming USD, adjust as necessary
                    Status = "Created",  // Initial status, should be updated later
                    CreatedAt = DateTime.UtcNow // Store the creation timestamp
                };

                _dbContext.Payments.Add(paymentRecord);
                _dbContext.SaveChanges();

                return Ok(new { paymentId = payment.id, approvalUrl = payment.GetApprovalUrl() });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing the payment.", details = ex.Message });
            }
        }



        [HttpGet("status/{paymentId}")]
        public async Task<IActionResult> GetPaymentStatus(string paymentId)
        {
            var payment = await _dbContext.Payments.FirstOrDefaultAsync(p => p.PaymentId == paymentId);
            if (payment == null)
            {
                return NotFound(new { message = "Payment not found" });
            }

            // Update payment status after verification (for simplicity, using "Completed" here)
            payment.Status = "Completed";
            await _dbContext.SaveChangesAsync();

            return Ok(payment);
        }

    }
}
