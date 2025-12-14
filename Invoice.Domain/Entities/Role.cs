using Invoice.Domain.Entities.Base;
using Invoice.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Invoice.Domain.Entities;

public class Role : IdentityRole<int>, IAuditableEntity
{
    public string Description { get; set; } = string.Empty;

    // Audit fields from IAuditableEntity
    public int? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
    public int? UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public bool IsDeleted { get; set; }

    // FK ids to users for audits
    public int? CreatedById { get; set; }
    public int? UpdatedById { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
    public virtual ICollection<RoleMenu> RoleMenus { get; set; } = new List<RoleMenu>();
}



