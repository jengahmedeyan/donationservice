using Application.Donations.Dtos;
using MediatR;

namespace Application.Donations.Commands.CancelDonationRequest;

/// <summary>
/// RequesterId is the authenticated caller (from their token), used for the
/// ownership check — NOT a value the client can freely pick to cancel others' requests.
/// Returns null when the request does not exist (controller maps to 404).
/// </summary>
public record CancelDonationRequestCommand(Guid Id, string RequesterId)
    : IRequest<DonationRequestDto?>;
