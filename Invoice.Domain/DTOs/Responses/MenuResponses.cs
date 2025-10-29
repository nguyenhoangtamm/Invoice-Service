using AutoMapper;
using Invoice.Domain.Common.Mappings;
using Invoice.Domain.Entities;

namespace Invoice.Domain.DTOs.Responses;

public class MenuResponse : IMapFrom<Menu>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public int Order { get; set; }
    public int? ParentId { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }

    public List<MenuResponse> Children { get; set; } = new();
}

public class MenuTreeResponse : IMapFrom<Menu>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public int Order { get; set; }
    public int? ParentId { get; set; }
    public bool HasPermission { get; set; }

    public List<MenuTreeResponse> Children { get; set; } = new();

    public void Mapping(AutoMapper.Profile profile)
    {
        profile.CreateMap<Menu, MenuTreeResponse>()
            .ForMember(dest => dest.HasPermission, opt => opt.Ignore());
    }
}

public class UserMenuResponse : IMapFrom<Menu>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? Url { get; set; }
    public bool? IsBlank { get; set; }
    public int? ParentId { get; set; }
    public int Order { get; set; }
    public List<UserMenuResponse> Children { get; set; } = new();

    public void Mapping(AutoMapper.Profile profile)
    {
        profile.CreateMap<Menu, UserMenuResponse>()
            .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Path))
            .ForMember(dest => dest.IsBlank, opt => opt.MapFrom(src => false)); // Default to false
    }
}

public class RoleMenuResponse : IMapFrom<RoleMenu>
{
    public int Id { get; set; }
    public int RoleId { get; set; }
    public int MenuId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string MenuName { get; set; } = string.Empty;

    public void Mapping(AutoMapper.Profile profile)
    {
        profile.CreateMap<RoleMenu, RoleMenuResponse>()
            .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name))
            .ForMember(dest => dest.MenuName, opt => opt.MapFrom(src => src.Menu.Name));
    }
}

