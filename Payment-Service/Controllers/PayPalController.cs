﻿using Microsoft.AspNetCore.Mvc;
using Payment_Service.Models;
using Payment_Service.Service;
using System.Threading.Tasks;

namespace Payment_Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PayPalController : ControllerBase
    {
        private readonly PayPalService _payPalService;

        public PayPalController(PayPalService payPalService)
        {
            _payPalService = payPalService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            var orderId = await _payPalService.CreateOrder(request.Amount, request.Currency);
            var approvalLink = await _payPalService.GetApprovalLink(orderId);

            return Ok(new
            {
                orderId = orderId,
                approveUrl = approvalLink
            });
        }

        [HttpPost("capture/{orderId}")]
        public async Task<IActionResult> CaptureOrder(string orderId)
        {
            var success = await _payPalService.CaptureOrder(orderId);

            if (success)
                return Ok(new { message = "Payment captured successfully." });
            else
                return BadRequest(new { message = "Failed to capture payment." });
        }
    }
}
