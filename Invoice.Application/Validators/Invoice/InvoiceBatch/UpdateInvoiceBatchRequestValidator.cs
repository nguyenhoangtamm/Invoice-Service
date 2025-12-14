using FluentValidation;
using Invoice.Domain.DTOs.Requests;

namespace Invoice.Application.Validators.Invoice.InvoiceBatch;

public class UpdateInvoiceBatchRequestValidator : AbstractValidator<UpdateInvoiceBatchRequest>
{
    public UpdateInvoiceBatchRequestValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);

        When(x => x.BatchId != null, () =>
        {
            RuleFor(x => x.BatchId).NotEmpty().MaximumLength(100);
        });

        When(x => x.Count.HasValue, () =>
        {
            RuleFor(x => x.Count.Value).GreaterThanOrEqualTo(0);
        });

        When(x => x.MerkleRoot != null, () =>
        {
            RuleFor(x => x.MerkleRoot).MaximumLength(256);
        });

        When(x => x.BatchCid != null, () =>
        {
            RuleFor(x => x.BatchCid).MaximumLength(200);
        });

        When(x => x.TxHash != null, () =>
        {
            RuleFor(x => x.TxHash).MaximumLength(200);
        });

        When(x => x.BlockNumber.HasValue, () =>
        {
            RuleFor(x => x.BlockNumber.Value).GreaterThanOrEqualTo(0);
        });

        When(x => x.Status.HasValue, () =>
        {
            RuleFor(x => x.Status.Value).IsInEnum();
        });
    }
}
