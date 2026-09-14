using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _2_Decorator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DecoratorController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public DecoratorController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var result = _orderService.GetOrder();

            return Ok(result);
        }
    }
}
