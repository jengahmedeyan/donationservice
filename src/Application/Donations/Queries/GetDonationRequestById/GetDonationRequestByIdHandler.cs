using Application.Common.Exceptions;
using Application.Donations.Dtos;
using Domain.Interfaces;
using MediatR;

namespace Application.Donations.Queries.GetDonationRequestById;

public class GetDonationRequestByIdHandler
    : IRequestHandler<GetDonationRequestByIdQuery, DonationRequestDto?>
{
    private readonly IDonationRepository _repository;

    public GetDonationRequestByIdHandler(IDonationRepository repository)
        => _repository = repository;

    public async Task<DonationRequestDto?> Handle(
        GetDonationRequestByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);

        // Not found -> null (controller maps to 404).
        if (entity is null)
            return null;

        // Object-level authorization: callers may only read their OWN requests.
        if (entity.RequesterId != request.RequesterId)
            throw new ForbiddenAccessException();

        return new DonationRequestDto(
            entity.Id,
            entity.RequesterName,
            entity.ItemNeeded,
            entity.Location,
            entity.Status.ToString(),
            entity.CreatedAt);
    }
}
