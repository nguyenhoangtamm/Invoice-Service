using FluentValidation;
using Invoice.Domain.DTOs.Requests;

namespace Invoice.Application.Validators.Menu;

public class CreateMenuRequestValidator : AbstractValidator<CreateMenuRequest>
{
    public CreateMenuRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Path)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Icon)
            .MaximumLength(50);

        RuleFor(x => x.Order)
            .GreaterThanOrEqualTo(0);

        When(x => x.ParentId.HasValue, () =>
        {
            RuleFor(x => x.ParentId.Value)
                .GreaterThan(0);
        });
    }
}

