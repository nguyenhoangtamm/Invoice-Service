using FluentValidation;
using Invoice.Domain.DTOs.Requests;

namespace Invoice.Application.Validators.Invoice.ApiKey;

public class UpdateApiKeyRequestValidator : AbstractValidator<UpdateApiKeyRequest>
{
    public UpdateApiKeyRequestValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);

        When(x => x.Name != null, () =>
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        });

        When(x => x.OrganizationId.HasValue, () =>
        {
            RuleFor(x => x.OrganizationId.Value).GreaterThan(0);
        });
    }
}
