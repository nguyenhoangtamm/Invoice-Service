using Invoice.Domain.Common.Mappings;
using Invoice.Domain.Entities;
using Invoice.Domain.Enums;

namespace Invoice.Domain.DTOs.Responses;

public class UserDto : IMapFrom<User>
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public UserStatus Status { get; set; }
}

public class GetUserDto : IMapFrom<User>
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Fullname { get; set; } = string.Empty;
    public UserStatus Status { get; set; }
    public int RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Gender { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? Address { get; set; }
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}

public class GetAllUsersDto : IMapFrom<User>
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public UserStatus Status { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string Fullname { get; set; } = string.Empty;
    public DateTime? CreatedDate { get; set; }
}

public class GetUsersWithPaginationDto : IMapFrom<User>
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public UserStatus Status { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string Fullname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime? CreatedDate { get; set; }
    public string? Phone { get; set; }
}

public class DashboardStatsDto
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int InactiveUsers { get; set; }
}

public class DashboardStatsResponse
{
    public string Id { get; set; } = string.Empty;
    public int TotalInvoices { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TotalCustomers { get; set; }
    public decimal AvgInvoiceValue { get; set; }
    public decimal MonthlyGrowth { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

