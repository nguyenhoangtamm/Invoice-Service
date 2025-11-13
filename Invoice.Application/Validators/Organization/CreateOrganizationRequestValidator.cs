using FluentValidation;
using Invoice.Domain.DTOs.Requests;

namespace Invoice.Application.Validators.Organization;

public class CreateOrganizationRequestValidator : AbstractValidator<CreateOrganizationRequest>
{
    public CreateOrganizationRequestValidator()
    {
        RuleFor(x => x.OrganizationName).NotEmpty().WithMessage("Organization name is required").MaximumLength(200);
        RuleFor(x => x.OrganizationTaxId).MaximumLength(50);
        RuleFor(x => x.OrganizationAddress).MaximumLength(500);
        RuleFor(x => x.OrganizationPhone).MaximumLength(30);
        RuleFor(x => x.OrganizationEmail).EmailAddress().When(x => !string.IsNullOrEmpty(x.OrganizationEmail)).MaximumLength(200);
        RuleFor(x => x.OrganizationBankAccount).MaximumLength(100);
    }
}
