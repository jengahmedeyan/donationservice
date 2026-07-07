using Application.Donations.Dtos;
using MediatR;

namespace Application.Donations.Queries.GetMyDonationRequests;

/// <summary>
/// RequesterId is the authenticated caller (from their token), used to scope the
/// results to their OWN requests — NOT a value the client can freely pick to read
/// others' data. There is deliberately no other field to spoof.
/// </summary>
public record GetMyDonationRequestsQuery(string RequesterId)
    : IRequest<IReadOnlyList<DonationRequestDto>>;
