using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Payment_Service.Models;
using Payment_Service.Service;
using System;
using System.Threading.Tasks;


namespace Payment_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayPalController : ControllerBase
    {
        private readonly PayPalService _payPalService;
        private readonly PaymentsDbContext _context;

        public PayPalController(PayPalService payPalService, PaymentsDbContext context)
        {
            _payPalService = payPalService;
            _context = context;
        }

        
        [HttpPost("create-order")]
        public async Task<IActionResult> CreateOrder(decimal amount, string currency)
        {
            try
            {
                // Create PayPal order
                var orderId = await _payPalService.CreatePayPalOrder(amount, currency);

                return Ok(new { message = "Order created successfully.", orderId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the order.", error = ex.Message });
            }
        }

        
        [HttpPost("capture-payment")]
        public async Task<IActionResult> CapturePayment(string orderId)
        {
            try
            {
                
                var paymentStatus = await _payPalService.CapturePayPalPayment(orderId);

                if (paymentStatus == "COMPLETED")
                {
                    
                    var transaction = new PaymentPaypalTransaction
                    {
                        PaymentId = Guid.NewGuid(),
                        PayPalTransactionId = orderId,
                        TransactionStatus = paymentStatus,
                        TransactionAmount = 0.0m, 
                        TransactionCurrency = "USD",
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.PaymentPaypalTransactions.Add(transaction);
                    await _context.SaveChangesAsync();

                    return Ok(new { message = "Payment captured successfully.", transaction });
                }

                return BadRequest(new { message = "Payment capture failed." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while capturing the payment.", error = ex.Message });
            }
        }
    }
}
