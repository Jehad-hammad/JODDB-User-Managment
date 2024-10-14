using Domain.DTO;
using Microsoft.AspNetCore.Mvc;

namespace JODDB_User_Managment.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        public UserController()
        {
            
        }

        [HttpPost]
        public async Task<IActionResult> GetUsers(BaseSearch request)
        {
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UploadExcel([FromForm] IFormFile file)
        {
            return Ok();
        }
    }
}
