using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Invoice.Domain.Common.Mappings;
using Invoice.Domain.Entities;

namespace Invoice.Domain.DTOs.Requests;

public class CreateMenuRequest : IMapFrom<Menu>
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Path { get; set; } = string.Empty;

    [StringLength(50)]
    public string Icon { get; set; } = string.Empty;

    public int Order { get; set; }

    public int? ParentId { get; set; }

    public void Mapping(AutoMapper.Profile profile)
    {
        profile.CreateMap<CreateMenuRequest, Menu>();
    }
}

public class UpdateMenuRequest : IMapFrom<Menu>
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Path { get; set; } = string.Empty;

    [StringLength(50)]
    public string Icon { get; set; } = string.Empty;

    public int Order { get; set; }

    public int? ParentId { get; set; }

    public void Mapping(AutoMapper.Profile profile)
    {
        profile.CreateMap<UpdateMenuRequest, Menu>();
    }
}

public class AssignMenuToRoleRequest
{
    [Required]
    public int RoleId { get; set; }

    [Required]
    public List<int> MenuIds { get; set; } = new();
}

public record GetMenuWithPagination
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Keyword { get; set; }
}

