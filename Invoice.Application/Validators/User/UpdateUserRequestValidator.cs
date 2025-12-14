using FluentValidation;
using Invoice.Domain.DTOs.Requests;
using Invoice.Domain.Enums;

namespace Invoice.Application.Validators.User;

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {

        When(x => x.Username != null, () =>
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .MaximumLength(50);
        });

        When(x => x.Email != null, () =>
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(100);
        });

        When(x => x.FullName != null, () =>
        {
            RuleFor(x => x.FullName)
                .NotEmpty()
                .MaximumLength(50);
        });

        When(x => x.Gender.HasValue, () =>
        {
            RuleFor(x => x.Gender.Value)
                .IsInEnum();
        });

        When(x => x.RoleId.HasValue, () =>
        {
            RuleFor(x => x.RoleId.Value)
                .GreaterThan(0);
        });

        When(x => x.Status.HasValue, () =>
        {
            RuleFor(x => x.Status.Value)
                .IsInEnum();
        });
    }
}

