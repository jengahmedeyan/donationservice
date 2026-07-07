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
    Task UpdateAsync(DonationRequest request, CancellationToken ct = default);
}
