using FluentValidation;
using Invoice.Domain.DTOs.Requests;

namespace Invoice.Application.Validators.Invoice.InvoiceLine;

public class UpdateInvoiceLineRequestValidator : AbstractValidator<UpdateInvoiceLineRequest>
{
    public UpdateInvoiceLineRequestValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        When(x => x.InvoiceId.HasValue, () => RuleFor(x => x.InvoiceId.Value).GreaterThan(0));
        When(x => x.LineNumber.HasValue, () => RuleFor(x => x.LineNumber.Value).GreaterThan(0));
        When(x => x.Name != null, () => RuleFor(x => x.Name).MaximumLength(1000));
        When(x => x.Unit != null, () => RuleFor(x => x.Unit).MaximumLength(50));
        When(x => x.Quantity.HasValue, () => RuleFor(x => x.Quantity.Value).GreaterThan(0));
        When(x => x.UnitPrice.HasValue, () => RuleFor(x => x.UnitPrice.Value).GreaterThanOrEqualTo(0));
        When(x => x.Discount.HasValue, () => RuleFor(x => x.Discount.Value).GreaterThanOrEqualTo(0));
        When(x => x.TaxRate.HasValue, () => RuleFor(x => x.TaxRate.Value).GreaterThanOrEqualTo(0));
        When(x => x.TaxAmount.HasValue, () => RuleFor(x => x.TaxAmount.Value).GreaterThanOrEqualTo(0));
        When(x => x.LineTotal.HasValue, () => RuleFor(x => x.LineTotal.Value).GreaterThanOrEqualTo(0));
    }
}
