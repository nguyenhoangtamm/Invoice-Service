using FluentValidation;
using Invoice.Domain.DTOs.Requests;

namespace Invoice.Application.Validators.Role;

public class UpdateRoleRequestValidator : AbstractValidator<UpdateRoleRequest>
{
    public UpdateRoleRequestValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Description)
            .MaximumLength(200);
    }
}

