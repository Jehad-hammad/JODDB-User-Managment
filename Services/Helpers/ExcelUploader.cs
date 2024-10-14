using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using OfficeOpenXml;
using Repository.Context;
using System.Diagnostics;

namespace Services.Helpers
{
    public class ExcelUploader : IExcelUploader
    {
        private readonly IServiceProvider _serviceProvider;
        public ExcelUploader(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public async Task<bool> ProcessExcel(IFormFile file)
        {
            var stopwatch = Stopwatch.StartNew();
            Console.WriteLine("Starting the user upload process...");

            var users = ReadExcelFile(file);

            int batchSize = 10000;
            var userChunks = SplitIntoChunks(users, batchSize);

            int maxConcurrentTasks = Environment.ProcessorCount * 2; 
            var _semaphore = new SemaphoreSlim(maxConcurrentTasks);

            var tasks = new List<Task>();

            foreach (var chunk in userChunks)
            {
                tasks.Add(Task.Run(async () =>
                {
                    await _semaphore.WaitAsync();
                    try
                    {
                        var chunkStopwatch = Stopwatch.StartNew(); 
                        using (var scope = _serviceProvider.CreateScope()) 
                        {
                            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
                            await BulkInsertUsers(chunk, dbContext);
                        }
                        chunkStopwatch.Stop();
                        Console.WriteLine($"Chunk processed in {chunkStopwatch.Elapsed.TotalSeconds} seconds.");
                    }
                    finally
                    {
                        _semaphore.Release();
                    }
                }));
            }

            await Task.WhenAll(tasks);

            stopwatch.Stop();
            Console.WriteLine("-------------*******************------------------************************------------------------***********");
            Console.WriteLine();
            Console.WriteLine($"Total user upload process completed in {stopwatch.Elapsed.TotalSeconds} seconds.");
            Console.WriteLine();
            Console.WriteLine("-------------*******************------------------************************------------------------***********");

            return true;
        }

        private async Task BulkInsertUsers(List<ApplicationUser> users, ApplicationContext dbContext)
        {
            dbContext.ChangeTracker.AutoDetectChangesEnabled = false;

            await dbContext.ApplicationUsers.AddRangeAsync(users);

            await dbContext.SaveChangesAsync();

            dbContext.ChangeTracker.AutoDetectChangesEnabled = true;
        }

        private List<ApplicationUser> ReadExcelFile(IFormFile file)
        {
            var users = new List<ApplicationUser>();

            using (var package = new ExcelPackage(file.OpenReadStream()))
            {
                var worksheet = package.Workbook.Worksheets[0]; 
                int rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++) 
                {
                    var user = new ApplicationUser
                    {
                        FullName = worksheet.Cells[row, 2].Text,
                        Email = worksheet.Cells[row, 3].Text,
                        UserName = worksheet.Cells[row, 3].Text,
                        PhoneNumber = worksheet.Cells[row, 4].Text,
                        PasswordHash =  "P@ssw0rd"
                    };
                    users.Add(user);
                }
            }

            return users;
        }

        private static List<List<T>> SplitIntoChunks<T>(List<T> source, int chunkSize)
        {
            return source
                .Select((value, index) => new { Index = index, Value = value })
                .GroupBy(x => x.Index / chunkSize)
                .Select(group => group.Select(x => x.Value).ToList())
                .ToList();
        }
    }
}
