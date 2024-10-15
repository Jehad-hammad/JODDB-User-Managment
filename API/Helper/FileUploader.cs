namespace API.Helpers
{
    public class FileUploader
    {

        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileUploader(IWebHostEnvironment webHostEnvironment )
        {
            _webHostEnvironment = webHostEnvironment;            
            
        }

        public async Task<string> UploadFile(IFormFile File, string directory , string requestUrl)
        {
            string FilePath = Path.Combine(_webHostEnvironment.WebRootPath + $"\\{directory}\\");


            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(File.FileName);

            var fullPath = Path.Combine(FilePath, fileName);

            await SaveFile(File, fullPath);
        
            return $"{requestUrl}/{directory}/{fileName}";
        }



        private async Task SaveFile(IFormFile file, string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                await file.CopyToAsync(fs);
            }
        }
    }
}

