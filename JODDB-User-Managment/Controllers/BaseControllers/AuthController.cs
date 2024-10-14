using Domain.DTO;
using Microsoft.AspNetCore.Mvc;

namespace JODDB_User_Managment.Controllers.BaseControllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public AuthController()
        {
            
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> RegistNewUser(UserRegistrationRequest request)
        {
            return Ok();
        }
    }
}
