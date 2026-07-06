using Application.Donations.Dtos;
using MediatR;

namespace Application.Donations.Commands.SubmitDonationRequest;

/// <summary>
/// In production, RequesterId should be bound from the authenticated user's
/// claims in the controller — not trusted from the request body. It is included
/// on the command so the handler can stamp ownership.
/// </summary>
public record SubmitDonationRequestCommand(
    string RequesterId,
    string RequesterName,
    string ItemNeeded,
    string Location) : IRequest<DonationRequestDto>;
