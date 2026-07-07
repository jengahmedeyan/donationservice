using Application.Donations.Dtos;
using Domain.Interfaces;
using MediatR;

namespace Application.Donations.Queries.GetMyDonationRequests;

public class GetMyDonationRequestsHandler
    : IRequestHandler<GetMyDonationRequestsQuery, IReadOnlyList<DonationRequestDto>>
{
    private readonly IDonationRepository _repository;

    public GetMyDonationRequestsHandler(IDonationRepository repository)
        => _repository = repository;

    public async Task<IReadOnlyList<DonationRequestDto>> Handle(
        GetMyDonationRequestsQuery request, CancellationToken cancellationToken)
    {
        // Scoped to the caller at the data layer — this only ever returns their own rows.
        var entities = await _repository.GetByRequesterIdAsync(
            request.RequesterId, cancellationToken);

        // Empty list is a valid result (200) — no not-found, no throw.
        return entities
            .Select(entity => new DonationRequestDto(
                entity.Id,
                entity.RequesterName,
                entity.ItemNeeded,
                entity.Location,
                entity.Status.ToString(),
                entity.CreatedAt))
            .ToList();
    }
}
