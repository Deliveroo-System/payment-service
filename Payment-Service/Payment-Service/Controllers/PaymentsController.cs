using Microsoft.AspNetCore.Mvc;
using Payment_Service.Service;
using System.Threading.Tasks;

namespace Payment_Service.Controllers
{
    [Route("api/payments")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly PayPalService _payPalService;

        public PaymentsController(PayPalService payPalService)
        {
            _payPalService = payPalService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment(decimal amount)
        {
            var payment = await _payPalService.CreatePayment(amount, "USD", "https://yourfrontend.com/success", "https://yourfrontend.com/cancel");

            return Ok(new { paymentId = payment.id, approvalUrl = payment.GetApprovalUrl() });
        }
    }
}
