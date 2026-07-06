namespace Domain.Entities;

/// <summary>
/// A request from a community member for a donated item.
/// RequesterId is the owner (the authenticated subject who created it) and is
/// used for object-level authorization — it is never exposed in API responses.
/// </summary>
public class DonationRequest
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string RequesterId { get; init; } = string.Empty;   // owner / auth subject (not returned)
    public string RequesterName { get; init; } = string.Empty; // PII
    public string ItemNeeded { get; init; } = string.Empty;
    public string Location { get; init; } = string.Empty;      // PII
    public DonationStatus Status { get; private set; } = DonationStatus.Open;
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;

    public void MarkFulfilled() => Status = DonationStatus.Fulfilled;
    public void Cancel() => Status = DonationStatus.Cancelled;
}

public enum DonationStatus
{
    Open,
    Fulfilled,
    Cancelled
}
