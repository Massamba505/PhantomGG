namespace PhantomGG.Models.DTOs;

public class PaginatedResponse<T>
{
    public IEnumerable<T> Data { get; set; } = new List<T>();
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalRecords { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }

    public PaginatedResponse()
    {
    }

    public PaginatedResponse(IEnumerable<T> data, int pageNumber, int pageSize, int totalRecords)
    {
        Data = data;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalRecords = totalRecords;
        TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
        HasNextPage = pageNumber < TotalPages;
        HasPreviousPage = pageNumber > 1;
    }
}

public class PaginatedResult<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int TotalRecords { get; set; }

    public PaginatedResult(IEnumerable<T> items, int totalRecords)
    {
        Items = items;
        TotalRecords = totalRecords;
    }
}
