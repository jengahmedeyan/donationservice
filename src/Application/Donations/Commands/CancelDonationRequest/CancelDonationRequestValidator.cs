using FluentValidation;

namespace Application.Donations.Commands.CancelDonationRequest;

public class CancelDonationRequestValidator : AbstractValidator<CancelDonationRequestCommand>
{
    public CancelDonationRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.RequesterId).NotEmpty();
    }
}
