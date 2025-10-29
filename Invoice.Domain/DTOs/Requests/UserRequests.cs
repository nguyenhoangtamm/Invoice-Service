using Invoice.Domain.Enums;

namespace Invoice.Domain.DTOs.Requests;

public record CreateUserRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Gender Gender { get; set; } = Gender.Other;
    public int RoleId { get; set; }
}

public record UpdateUserRequest
{
    public int Id { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public Gender? Gender { get; set; }
    public int? RoleId { get; set; }
    public UserStatus? Status { get; set; }
}

public record GetUsersWithPaginationQuery
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Keyword { get; set; }
}

public record UpdateUserWithIdRequest
{
    public int Id { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public int? RoleId { get; set; }
    public UserStatus? Status { get; set; }
}

public record DeleteUserRequest
{
    public int Id { get; set; }
}


