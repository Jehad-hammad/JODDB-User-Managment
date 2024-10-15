namespace Domain.DTO
{
    public class BaseSearch
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }  
        public string? MobileNumber { get; set; }
    }
}
    