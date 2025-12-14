using FluentValidation;
using Invoice.Domain.DTOs.Requests;

namespace Invoice.Application.Validators.Invoice;

public class CreateInvoiceRequestValidator : AbstractValidator<CreateInvoiceRequest>
{
    public CreateInvoiceRequestValidator()
    {
        RuleFor(x => x.InvoiceNumber).MaximumLength(50);
        RuleFor(x => x.FormNumber).MaximumLength(50);
        RuleFor(x => x.Serial).MaximumLength(50);

        RuleFor(x => x.SellerName).MaximumLength(200);
        RuleFor(x => x.SellerTaxId).MaximumLength(50);
        RuleFor(x => x.SellerAddress).MaximumLength(500);
        RuleFor(x => x.SellerPhone).MaximumLength(30);
        RuleFor(x => x.SellerEmail).EmailAddress().When(x => !string.IsNullOrEmpty(x.SellerEmail)).MaximumLength(200);

        RuleFor(x => x.CustomerName).MaximumLength(200);
        RuleFor(x => x.CustomerTaxId).MaximumLength(50);
        RuleFor(x => x.CustomerAddress).MaximumLength(500);
        RuleFor(x => x.CustomerPhone).MaximumLength(30);
        RuleFor(x => x.CustomerEmail).EmailAddress().When(x => !string.IsNullOrEmpty(x.CustomerEmail)).MaximumLength(200);

        RuleFor(x => x.Currency).MaximumLength(10);
        RuleFor(x => x.Note).MaximumLength(4000);

        When(x => x.SubTotal.HasValue, () =>
        {
            RuleFor(x => x.SubTotal.Value).GreaterThanOrEqualTo(0);
        });

        When(x => x.TaxAmount.HasValue, () =>
        {
            RuleFor(x => x.TaxAmount.Value).GreaterThanOrEqualTo(0);
        });

        When(x => x.DiscountAmount.HasValue, () =>
        {
            RuleFor(x => x.DiscountAmount.Value).GreaterThanOrEqualTo(0);
        });

        When(x => x.TotalAmount.HasValue, () =>
        {
            RuleFor(x => x.TotalAmount.Value).GreaterThanOrEqualTo(0);
        });
    }
}
