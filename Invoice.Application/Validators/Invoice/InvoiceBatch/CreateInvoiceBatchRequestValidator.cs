using FluentValidation;
using Invoice.Domain.DTOs.Requests;

namespace Invoice.Application.Validators.Invoice.InvoiceBatch;

public class CreateInvoiceBatchRequestValidator : AbstractValidator<CreateInvoiceBatchRequest>
{
    public CreateInvoiceBatchRequestValidator()
    {
        RuleFor(x => x.BatchId)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Count)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.MerkleRoot)
            .MaximumLength(256)
            .When(x => !string.IsNullOrEmpty(x.MerkleRoot));

        RuleFor(x => x.BatchCid)
            .MaximumLength(200)
            .When(x => !string.IsNullOrEmpty(x.BatchCid));

        RuleFor(x => x.TxHash)
            .MaximumLength(200)
            .When(x => !string.IsNullOrEmpty(x.TxHash));

        When(x => x.BlockNumber.HasValue, () =>
        {
            RuleFor(x => x.BlockNumber.Value).GreaterThanOrEqualTo(0);
        });

        RuleFor(x => x.Status).IsInEnum();

        // Require at least one invoice id when creating a batch
        RuleFor(x => x.InvoiceIds)
            .NotNull()
            .Must(list => list != null && list.Count > 0)
            .WithMessage("InvoiceIds must contain at least one invoice id.");
    }
}
