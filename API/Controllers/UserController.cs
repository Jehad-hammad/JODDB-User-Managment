using API.Helpers;
using Domain.DTO;
using Domain.DTO.Others;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
        private readonly FileUploader _fileUploader;
        public UserController(IServiceUnitOfWork serviceUnitOfWork , IExcelUploader excelUploader, IWebHostEnvironment webHostEnvironment)
        {
            _serviceUnitOfWork = serviceUnitOfWork;
            _excelUploader = excelUploader;
            _fileUploader = new FileUploader(webHostEnvironment);

        }

        /// <summary>
        /// Retrieves a paginated list of users based on the provided search criteria.
        /// </summary>
        /// <param name="request">A search request containing pagination options and optional filters such as name, email, and mobile number.</param>
        /// <returns>
        /// A 200 OK response containing the paginated list of users if successful.
        /// </returns>
        /// <remarks>
        /// Example request:
        /// 
        /// <code>
        /// POST /api/users/getlist
        /// {
        ///     "pageSize": 10,
        ///     "pageNumber": 1,
        ///     "name": "John",
        ///     "email": "john@example.com",
        ///     "mobileNumber": "1234567890"
        /// }
        /// </code>
        ///
        /// - The API allows searching for users by name, email, or mobile number.
        /// - The result is paginated based on the provided `PageSize` and `PageNumber`.
        /// </remarks>
        /// <response code="200">Returns the list of users based on the search criteria.</response>
        /// <response code="400">Returns a validation error if the search criteria are invalid.</response>

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

        /// <summary>
        /// Registers a new user with the provided details and uploads an optional profile picture.
        /// </summary>
        /// <param name="request">The user's registration details, including name, email, mobile number, password, and an optional file (profile picture).</param>
        /// <returns>
        /// A 200 OK response if the user registration is successful.
        /// </returns>
        /// <remarks>
        /// Example request:
        ///<code>
        /// POST /api/auth/userregistration
        /// {
        ///     "name": "Jane Doe",
        ///     "email": "jane.doe@example.com",
        ///     "mobileNumber": "9876543210",
        ///     "password": "SecurePassword123",
        ///     "file": file (optional)
        /// }
        ///
        /// </code>
        /// - The API registers a new user and optionally uploads a profile picture.
        /// - If a file is uploaded, the API will generate a URL for the file and save it.
        /// </remarks>
        /// <response code="200">Returns a successful response upon user registration.</response>
        /// <response code="400">Returns a validation error if the registration data is invalid.</response>

        [HttpPost]
        public async Task<IActionResult> UserRegistration([FromForm] UserRegistrationRequest request)
        {
            try
            {
                if(request.file != null)
                {
                    var requestUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.PathBase}";

                    request.filePath = await _fileUploader.UploadFile(request.file , "users" , requestUrl);
                }
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

        /// <summary>
        /// Retrieves detailed information about a specific user by their ID.
        /// </summary>
        /// <param name="UserId">The unique identifier of the user.</param>
        /// <returns>
        /// A 200 OK response containing the user details if the user exists.
        /// </returns>
        /// <remarks>
        /// Example request:
        /// 
        /// <code>
        /// GET /api/users/1
        ///
        /// </code>
        /// - The API fetches the details of a user with the given UserId.
        /// </remarks>
        /// <response code="200">Returns the user details for the specified UserId.</response>
        /// <response code="400">Returns a validation error if the user is not found.</response>

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

        /// <summary>
        /// Uploads an Excel file and processes its content.
        /// </summary>
        /// <param name="request">A request containing the Excel file to be uploaded and processed.</param>
        /// <returns>
        /// A 200 OK response if the Excel file is processed successfully.
        /// </returns>
        /// <remarks>
        /// Example request:
        ///
        /// <code>
        /// POST /api/excel/upload
        /// {
        ///     "excelFile": file
        /// }
        ///
        /// </code>
        /// - The API processes the uploaded Excel file, typically for bulk user import or other data processing purposes.
        /// </remarks>
        /// <response code="200">Returns a successful response if the file is processed successfully.</response>
        /// <response code="400">Returns a validation error if the file is not valid or processing fails.</response>

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
