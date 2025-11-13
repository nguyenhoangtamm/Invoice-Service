using FluentValidation;
using Invoice.Domain.DTOs.Requests;

namespace Invoice.Application.Validators.Invoice.InvoiceLine;

public class CreateInvoiceLineRequestValidator : AbstractValidator<CreateInvoiceLineRequest>
{
    public CreateInvoiceLineRequestValidator()
    {
        RuleFor(x => x.InvoiceId).GreaterThan(0);
        RuleFor(x => x.LineNumber).GreaterThan(0);
        RuleFor(x => x.Description).MaximumLength(1000);
        RuleFor(x => x.Unit).MaximumLength(50);
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0);
        When(x => x.Discount.HasValue, () => RuleFor(x => x.Discount.Value).GreaterThanOrEqualTo(0));
        When(x => x.TaxRate.HasValue, () => RuleFor(x => x.TaxRate.Value).GreaterThanOrEqualTo(0));
        When(x => x.TaxAmount.HasValue, () => RuleFor(x => x.TaxAmount.Value).GreaterThanOrEqualTo(0));
        RuleFor(x => x.LineTotal).GreaterThanOrEqualTo(0);
    }
}
