using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _5_Factory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        [HttpPost]
        public IActionResult Send(string type, string message)
        {
            var factory = new NotificationFactory();

            var notification = factory.Create(type);

            notification.Send(message);

            return Ok();
        }
    }
}
