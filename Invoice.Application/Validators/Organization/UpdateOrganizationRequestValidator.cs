using FluentValidation;
using Invoice.Domain.DTOs.Requests;

namespace Invoice.Application.Validators.Organization;

public class UpdateOrganizationRequestValidator : AbstractValidator<UpdateOrganizationRequest>
{
    public UpdateOrganizationRequestValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.OrganizationName).MaximumLength(200).When(x => x.OrganizationName != null);
        RuleFor(x => x.OrganizationTaxId).MaximumLength(50);
        RuleFor(x => x.OrganizationAddress).MaximumLength(500);
        RuleFor(x => x.OrganizationPhone).MaximumLength(30);
        RuleFor(x => x.OrganizationEmail).EmailAddress().When(x => !string.IsNullOrEmpty(x.OrganizationEmail)).MaximumLength(200);
        RuleFor(x => x.OrganizationBankAccount).MaximumLength(100);
    }
}
