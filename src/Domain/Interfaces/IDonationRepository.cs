using Domain.Entities;

namespace Domain.Interfaces;

/// <summary>
/// Domain-owned persistence contract. The concrete implementation lives in
/// Infrastructure — the only layer allowed to talk to a data store.
/// </summary>
public interface IDonationRepository
{
    Task<DonationRequest> AddAsync(DonationRequest request, CancellationToken ct = default);
    Task<DonationRequest?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Returns only the requests owned by <paramref name="requesterId"/>. Ownership
    /// filtering is enforced here so callers can never receive another user's rows.
    /// Empty when the requester has none — never null.
    /// </summary>
    Task<IReadOnlyList<DonationRequest>> GetByRequesterIdAsync(
        string requesterId, CancellationToken ct = default);

    Task UpdateAsync(DonationRequest request, CancellationToken ct = default);
}
