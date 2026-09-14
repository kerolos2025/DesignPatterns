using _4_Strategy.Models;
using _4_Strategy.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _4_Strategy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StrategyController : ControllerBase
    {
        private readonly IServiceProvider _serviceProvider;

        public StrategyController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [HttpPost]
        public IActionResult Send(NotificationRequest request)
        {
            var strategy =
                _serviceProvider.GetRequiredKeyedService<INotificationStrategy>(
                    request.Type.ToLower());

            strategy.Send(request.Message);

            return Ok("Notification sent successfully");
        }
    }
}
