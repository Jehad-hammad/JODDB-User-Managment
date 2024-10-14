using Domain.DTO;
using Domain.DTO.Others;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.UnitOfWork;
using Services.Helpers;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IServiceUnitOfWork _serviceUnitOfWork;
        private readonly IExcelUploader _excelUploader;
        public UserController(IServiceUnitOfWork serviceUnitOfWork , IExcelUploader excelUploader)
        {
            _serviceUnitOfWork = serviceUnitOfWork;
            _excelUploader = excelUploader;
        }

        [HttpPost]
        public async Task<IActionResult> GetList([FromBody] BaseSearch request)
        {
            try
            {
                return Ok(await _serviceUnitOfWork.Users.Value.GetUserList(request));
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> UserRegistration([FromForm] UserRegistrationRequest request)
        {
            try
            {
                return Ok(await _serviceUnitOfWork.Auth.Value.RegistNewUser(request));
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet("{UserId}")]
        public async Task<IActionResult> GetUser(long UserId)
        {
            try
            {
                return Ok(await _serviceUnitOfWork.Users.Value.GetUserById(UserId));
            }
            catch (ValidationException ex)
            {

                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost , DisableRequestSizeLimit]
        public async Task<IActionResult> UploadExcel([FromForm] FileUploadRequest request)
        {
            try
            {
                return Ok(await _excelUploader.ProcessExcel(request.ExcelFile));

            }
            catch (Exception e)
            {

                throw;
            }
        }


    }
}
