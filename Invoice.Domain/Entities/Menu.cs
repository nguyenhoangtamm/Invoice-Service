using Invoice.Domain.Entities.Base;

namespace Invoice.Domain.Entities;

public class Menu : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public int Order { get; set; }
    public int? ParentId { get; set; }

    // Navigation properties
    public virtual Menu? Parent { get; set; }
    public virtual ICollection<Menu> Children { get; set; } = new List<Menu>();
    public virtual ICollection<RoleMenu> RoleMenus { get; set; } = new List<RoleMenu>();
}


