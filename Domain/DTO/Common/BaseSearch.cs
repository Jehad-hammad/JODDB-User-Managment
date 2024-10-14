namespace Domain.DTO
{
    public class BaseSearch
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public string? Name { get; set; }
    }
}
