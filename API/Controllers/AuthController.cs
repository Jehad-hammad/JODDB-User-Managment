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

        /// <summary>
        /// Logs in a user with the provided credentials and returns a JWT token upon successful login.
        /// </summary>
        /// <param name="request">The user's login request containing username and password.</param>
        /// <returns>
        /// A 200 OK response containing a <see cref="TokenResponseDTO"/> if login is successful, 
        /// or a 400 Bad Request response if validation fails.
        /// </returns>
        /// <remarks>
        /// Example request:
        /// 
        /// <code>
        /// POST /api/auth/login
        /// {
        ///     "username": "testuser",
        ///     "password": "Test@12345"
        /// }
        /// </code>
        /// 
        /// - The API expects a username and password in the request body.
        /// - If the login is successful, a JWT token is returned.
        /// - If the username or password is incorrect, a validation exception is thrown.
        /// </remarks>
        /// <response code="200">
        /// Returns the JWT token in the form of a <see cref="TokenResponseDTO"/>.
        /// </response>
        /// <response code="400">
        /// Returns a validation error message if the username or password is invalid.
        /// </response>
        /// <exception cref="ValidationException">
        /// Thrown if the user is not found or the password is incorrect.
        /// </exception>
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
