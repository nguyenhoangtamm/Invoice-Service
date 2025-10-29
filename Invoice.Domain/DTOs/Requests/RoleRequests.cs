namespace Invoice.Domain.DTOs.Requests;

public class CreateRoleRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class UpdateRoleRequest
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class GetRolesWithPaginationQuery
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
}

