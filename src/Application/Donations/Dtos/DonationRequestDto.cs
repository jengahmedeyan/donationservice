namespace Application.Donations.Dtos;

/// <summary>
/// The shape returned to callers. Note: RequesterId (the owner/auth subject) is
/// deliberately NOT included — it is internal and must not leak in responses.
/// </summary>
public record DonationRequestDto(
    Guid Id,
    string RequesterName,
    string ItemNeeded,
    string Location,
    string Status,
    DateTimeOffset CreatedAt);
