using FluentValidation;
using Invoice.Domain.DTOs.Requests;

namespace Invoice.Application.Validators.Menu;

public class AssignMenuToRoleRequestValidator : AbstractValidator<AssignMenuToRoleRequest>
{
    public AssignMenuToRoleRequestValidator()
    {
        RuleFor(x => x.RoleId)
            .GreaterThan(0);

        RuleFor(x => x.MenuIds)
            .NotEmpty();
    }
}

