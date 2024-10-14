using Domain.DTO;
using Microsoft.AspNetCore.Mvc;
using Service.UnitOfWork;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IServiceUnitOfWork _serviceUnitOfWork;
        public AuthController(IServiceUnitOfWork serviceUnitOfWork)
        {
            _serviceUnitOfWork = serviceUnitOfWork;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                return Ok(await _serviceUnitOfWork.Auth.Value.Login(request));

            }
            catch (ValidationException ex)
            {

                return BadRequest(ex.Message);
            }
            catch(Exception)
            {
                throw;
            }
        }

    }
}
