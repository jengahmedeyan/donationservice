using FluentValidation;

namespace Application.Donations.Commands.SubmitDonationRequest;

public class SubmitDonationRequestValidator : AbstractValidator<SubmitDonationRequestCommand>
{
    public SubmitDonationRequestValidator()
    {
        RuleFor(x => x.RequesterId).NotEmpty();
        RuleFor(x => x.RequesterName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ItemNeeded).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Location).NotEmpty().MaximumLength(300);
    }
}
