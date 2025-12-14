using Invoice.Domain.Common.Mappings;
using Invoice.Domain.Entities;
using Invoice.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoice.Domain.DTOs.Responses;

public class GetRolesWithPaginationDto : IMapFrom<Role>
{
    public int Id { get; set; }
    public string Description { get; set; }
    public string Name { get; set; }
    public string NormalizedName { get; set; }
}
