namespace AEIG.Domain;

public class DynamicQueryResult<TEntity>
{
    public DynamicQueryResult(int pageSize)
    {
        PageSize = pageSize;
    }

    public int PageSize { get; }
    public int PageNumber { get; set; }
    public int TotalRows { get; set; }
    public string? QueryString { get; set; }
    public List<TEntity> Result { get; set; } = new();
}
