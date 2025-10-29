namespace Invoice.Domain.Shares;
public class PaginatedResult<T> : Result<T>
{
    public PaginatedResult(List<T> data)
    {
        Data = data;
    }

    public PaginatedResult(bool succeeded, List<T> data = default, string message = default, int count = 0,
        int pageNumber = 1, int pageSize = 10)
    {
        Data = data;
        Succeeded = succeeded;
        Message = message;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        TotalCount = count;
    }

    public new List<T> Data { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public int PageSize { get; set; }

    public static PaginatedResult<T> Create(List<T> data, int count, int pageNumber, int pageSize)
    {
        return new PaginatedResult<T>(true, data, null, count, pageNumber, pageSize);
    }
}

