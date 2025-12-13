using FluentValidation;
using Invoice.Domain.DTOs.Requests;

namespace Invoice.Application.Validators.Role;

public class GetRolesWithPaginationQueryValidator : AbstractValidator<GetRolesWithPaginationQuery>
{
    public GetRolesWithPaginationQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100);

        RuleFor(x => x.KeyWord)
            .MaximumLength(100);
    }
}

