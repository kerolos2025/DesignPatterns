using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _1_Singleton.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SingletonController : ControllerBase
    {
        private readonly IRequestCounter _counter;

        public SingletonController(IRequestCounter counter)
        {
            _counter = counter;
        }

        [HttpGet]
        public IActionResult Get()
        {
            _counter.Increment();

            return Ok(new
            {
                Count = _counter.GetCount()
            });
        }
    }
}
