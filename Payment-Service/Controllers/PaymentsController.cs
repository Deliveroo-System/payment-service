using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Payment_Service.Models;
using Payment_Service.Service;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Payment_Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentsDbContext _context;
        private readonly PayPalService _paypalService;

        public PaymentController(PaymentsDbContext context, PayPalService paypalService)
        {
            _context = context;
            _paypalService = paypalService;
        }

        [HttpPost("paypal/create")]
        public async Task<IActionResult> CreatePaypalPayment([FromBody] PaypalPaymentRequest request)
        {
            try
            {
               
                Guid orderId = Guid.NewGuid(); 
                Guid userId = Guid.NewGuid(); 

                // Simulate PayPal order creation
                var paymentId = Guid.NewGuid();
                var paypalTransaction = new PaymentPaypalTransaction
                {
                    TransactionId = Guid.NewGuid(),
                    PayPalTransactionId = orderId.ToString(),  // Store the orderId as a string if needed here
                    TransactionStatus = "PENDING",
                    TransactionAmount = request.Amount,
                    TransactionCurrency = request.Currency ?? "USD",
                    CreatedAt = DateTime.UtcNow
                };

                var payment = new Payment
                {
                    PaymentId = paymentId,
                    OrderId = orderId,  // Use Guid here
                    UserId = userId,    // Use Guid here
                    TotalAmount = request.Amount,
                    Currency = request.Currency ?? "USD",
                    PaymentMethod = "PAYPAL",
                    PaymentStatus = "PENDING",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    PaypalTransactions = new List<PaymentPaypalTransaction> { paypalTransaction }
                };

                // Save payment to the database
                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                // Return orderId and approval link (simulated)
                return Ok(new { orderId, approvalLink = "https://www.paypal.com/approval-link" });
            }
            catch (DbUpdateException dbEx)
            {
                return BadRequest(new { error = dbEx.Message, inner = dbEx.InnerException?.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        

        [HttpPost("paypal/capture/{orderId}")]
        public async Task<IActionResult> CapturePaypalPayment(string orderId)
        {
            try
            {
                // Simulate PayPal capture (success)
                var success = true; // Replace with actual capture logic
                if (!success) return BadRequest("Payment capture failed.");

                var transaction = await _context.PaymentPaypalTransactions
                    .Include(t => t.Payment)
                    .FirstOrDefaultAsync(t => t.PayPalTransactionId == orderId);

                if (transaction != null)
                {
                    transaction.TransactionStatus = "SUCCESS";
                    transaction.Payment.PaymentStatus = "COMPLETED";
                    transaction.Payment.UpdatedAt = DateTime.UtcNow;

                    await _context.SaveChangesAsync();
                }

                return Ok(new { message = "Payment captured successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("cod")]
        public async Task<IActionResult> CreateCodPayment([FromBody] CodPaymentRequest request)
        {
            try
            {
                // Dummy data generation for orderId and userId
                Guid orderId = Guid.NewGuid();  // Use Guid instead of string
                Guid userId = Guid.NewGuid();   // Use Guid instead of string

                var paymentId = Guid.NewGuid();

                var payment = new Payment
                {
                    PaymentId = paymentId,
                    OrderId = orderId,  // Directly use Guid
                    UserId = userId,    // Directly use Guid
                    TotalAmount = request.TotalAmount,
                    Currency = request.Currency,
                    PaymentMethod = "CASH_ON_DELIVERY",
                    PaymentStatus = "PENDING",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CodTransaction = new PaymentCODTransaction
                    {
                        CodId = Guid.NewGuid(),
                        PaymentId = paymentId,
                        CodStatus = "PENDING",
                        CreatedAt = DateTime.UtcNow
                    }
                };

                // Save COD payment to the database
                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();



                return Ok(new { message = "COD payment created", paymentId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
