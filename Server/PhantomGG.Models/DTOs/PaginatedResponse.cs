namespace PhantomGG.Models.DTOs;

public class PagedResult<T>
{
    public IEnumerable<T> Data { get; set; } = new List<T>();
    public PaginationMeta Meta { get; set; } = new();

    public PagedResult(IEnumerable<T> data, int page, int pageSize, int totalRecords)
    {
        this.Data = data;
        Meta = new PaginationMeta
        {
            Page = page,
            PageSize = pageSize,
            TotalRecords = totalRecords,
            TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
            HasNextPage = page < (int)Math.Ceiling((double)totalRecords / pageSize),
            HasPreviousPage = page > 1
        };
    }
}

public class PaginationMeta
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalRecords { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
}