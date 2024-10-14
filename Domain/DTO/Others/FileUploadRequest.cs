using Microsoft.AspNetCore.Http;

namespace Domain.DTO.Others
{
    public class FileUploadRequest
    {
        public IFormFile ExcelFile { get; set; }
    }
}
