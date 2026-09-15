using _7_Builder.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _7_Builder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        [HttpPost]
        public IActionResult Create()
        {
            var user = new UserBuilder()
                .SetName("Kero")
                .SetEmail("kero@test.com")
                .MakeAdmin()
                .Build();

            return Ok(user);
        }
    }
}
