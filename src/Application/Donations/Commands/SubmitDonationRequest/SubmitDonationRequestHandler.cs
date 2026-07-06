using Application.Donations.Dtos;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.Donations.Commands.SubmitDonationRequest;

public class SubmitDonationRequestHandler
    : IRequestHandler<SubmitDonationRequestCommand, DonationRequestDto>
{
    private readonly IDonationRepository _repository;

    public SubmitDonationRequestHandler(IDonationRepository repository)
        => _repository = repository;

    public async Task<DonationRequestDto> Handle(
        SubmitDonationRequestCommand request, CancellationToken cancellationToken)
    {
        var entity = new DonationRequest
        {
            RequesterId = request.RequesterId,
            RequesterName = request.RequesterName,
            ItemNeeded = request.ItemNeeded,
            Location = request.Location
        };

        var saved = await _repository.AddAsync(entity, cancellationToken);

        return new DonationRequestDto(
            saved.Id,
            saved.RequesterName,
            saved.ItemNeeded,
            saved.Location,
            saved.Status.ToString(),
            saved.CreatedAt);
    }
}
