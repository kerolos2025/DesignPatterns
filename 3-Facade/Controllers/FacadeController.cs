using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _3_Facade.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacadeController : ControllerBase
    {
        private readonly IOrderFacade _orderFacade;

        public FacadeController(IOrderFacade orderFacade)
        {
            _orderFacade = orderFacade;
        }

        [HttpGet]
        public IActionResult CreateOrder()
        {
            _orderFacade.CreateOrder();

            return Ok("Order created");
        }
    }
}
