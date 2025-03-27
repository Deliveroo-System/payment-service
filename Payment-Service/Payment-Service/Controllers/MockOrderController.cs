using Microsoft.AspNetCore.Mvc;

namespace Payment_Service.Controllers
{
    public class MockOrderControllers
    {
        [ApiController]
        [Route("api/orders")]
        public class MockOrderController : ControllerBase
        {
            private static readonly Dictionary<Guid, OrderResponse> Orders = new()
    {
        { Guid.NewGuid(), new OrderResponse { OrderId = Guid.NewGuid(), UserId = Guid.NewGuid(), TotalAmount = 29.99m, OrderStatus = "PENDING" } }
    };

            [HttpGet("{orderId}")]
            public IActionResult GetOrder(Guid orderId)
            {
                if (!Orders.ContainsKey(orderId))
                {
                    return NotFound();
                }

                return Ok(Orders[orderId]);
            }
        }

        public class OrderResponse
        {
            public Guid OrderId { get; set; }
            public Guid UserId { get; set; }
            public decimal TotalAmount { get; set; }
            public string OrderStatus { get; set; }
        }
    }
}
