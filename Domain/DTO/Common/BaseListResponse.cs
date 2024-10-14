namespace Domain.DTO
{
    public class BaseListResponse<TEntity>
    {
        public List<TEntity> entities { get; set; }
        public int TotalCount { get; set; }
    }
}
