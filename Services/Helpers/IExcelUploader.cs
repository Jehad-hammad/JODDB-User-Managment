using Microsoft.AspNetCore.Http;

namespace Services.Helpers
{
    public interface IExcelUploader
    {
        Task<bool> ProcessExcel(IFormFile file);
    }
}
