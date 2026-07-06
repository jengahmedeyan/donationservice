using Application.Donations.Dtos;
using MediatR;

namespace Application.Donations.Queries.GetDonationRequestById;

/// <summary>
/// RequesterId is the authenticated caller (from their token), used for the
/// ownership check — NOT a value the client can freely pick to read others' data.
/// </summary>
public record GetDonationRequestByIdQuery(Guid Id, string RequesterId)
    : IRequest<DonationRequestDto?>;
