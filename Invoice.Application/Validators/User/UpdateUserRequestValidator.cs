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

        When(x => x.FirstName != null, () =>
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .MaximumLength(50);
        });

        When(x => x.LastName != null, () =>
        {
            RuleFor(x => x.LastName)
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

