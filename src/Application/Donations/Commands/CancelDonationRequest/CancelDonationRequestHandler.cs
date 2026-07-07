using Application.Common.Exceptions;
using Application.Donations.Dtos;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.Donations.Commands.CancelDonationRequest;

public class CancelDonationRequestHandler
    : IRequestHandler<CancelDonationRequestCommand, DonationRequestDto?>
{
    private readonly IDonationRepository _repository;

    public CancelDonationRequestHandler(IDonationRepository repository)
        => _repository = repository;

    public async Task<DonationRequestDto?> Handle(
        CancelDonationRequestCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);

        // Not found -> null (controller maps to 404).
        if (entity is null)
            return null;

        // Object-level authorization: callers may only cancel their OWN requests.
        if (entity.RequesterId != request.RequesterId)
            throw new ForbiddenAccessException();

        // Only an open request can be cancelled. Rejecting other states cleanly
        // (409) keeps already-cancelled/fulfilled requests from becoming 500s.
        if (entity.Status != DonationStatus.Open)
            throw new InvalidDonationStateException(
                $"Only an open request can be cancelled; this request is {entity.Status}.");

        entity.Cancel();
        await _repository.UpdateAsync(entity, cancellationToken);

        return new DonationRequestDto(
            entity.Id,
            entity.RequesterName,
            entity.ItemNeeded,
            entity.Location,
            entity.Status.ToString(),
            entity.CreatedAt);
    }
}
