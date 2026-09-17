using _8_Observer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _8_Observer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrdersController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> Create(int id)
        {
            await _orderService.CreateOrder(id);

            return Ok("Order created");
        }
    }
}
