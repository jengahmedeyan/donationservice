using System.Collections.Concurrent;
using Domain.Entities;
using Domain.Interfaces;

namespace Infrastructure.Repositories;

/// <summary>
/// In-memory store so the project runs with zero external dependencies.
/// Swap for an EF Core repository (Postgres) without touching Application/Domain.
/// </summary>
public class InMemoryDonationRepository : IDonationRepository
{
    private readonly ConcurrentDictionary<Guid, DonationRequest> _store = new();

    public Task<DonationRequest> AddAsync(DonationRequest request, CancellationToken ct = default)
    {
        _store[request.Id] = request;
        return Task.FromResult(request);
    }

    public Task<DonationRequest?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        _store.TryGetValue(id, out var request);
        return Task.FromResult(request);
    }

    public Task UpdateAsync(DonationRequest request, CancellationToken ct = default)
    {
        _store[request.Id] = request;
        return Task.CompletedTask;
    }
}
