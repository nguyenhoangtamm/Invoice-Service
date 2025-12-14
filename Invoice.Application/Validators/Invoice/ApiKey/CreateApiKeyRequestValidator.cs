using FluentValidation;
using Invoice.Domain.DTOs.Requests;

namespace Invoice.Application.Validators.Invoice.ApiKey;

public class CreateApiKeyRequestValidator : AbstractValidator<CreateApiKeyRequest>
{
    public CreateApiKeyRequestValidator()
    {
        RuleFor(x => x.Name).MaximumLength(200).When(x => !string.IsNullOrEmpty(x.Name));
        RuleFor(x => x.OrganizationId).GreaterThan(0);
        RuleFor(x => x.ExpirationDays).GreaterThan(0).When(x => x.ExpirationDays.HasValue);
    }
}
