using Domain.Entities;
using FluentAssertions;
using Infrastructure.Repositories;
using Xunit;

namespace Application.Tests.Donations;

public class InMemoryDonationRepositoryTests
{
    private static DonationRequest OwnedBy(string owner, string item) => new()
    {
        RequesterId = owner,
        RequesterName = "Ada Lovelace",
        ItemNeeded = item,
        Location = "Toronto"
    };

    [Fact]
    public async Task GetByRequesterIdAsync_ReturnsOnlyMatchingOwnersRows()
    {
        var repo = new InMemoryDonationRepository();
        await repo.AddAsync(OwnedBy("user-1", "Winter coat"));
        await repo.AddAsync(OwnedBy("user-1", "Blankets"));
        await repo.AddAsync(OwnedBy("user-2", "Groceries"));

        var result = await repo.GetByRequesterIdAsync("user-1");

        // The authorization-bypass trap: must exclude user-2's row entirely.
        result.Should().HaveCount(2);
        result.Should().OnlyContain(r => r.RequesterId == "user-1");
        result.Select(r => r.ItemNeeded).Should().BeEquivalentTo("Winter coat", "Blankets");
    }

    [Fact]
    public async Task GetByRequesterIdAsync_ReturnsEmpty_WhenOwnerHasNoRows()
    {
        var repo = new InMemoryDonationRepository();
        await repo.AddAsync(OwnedBy("user-2", "Groceries"));

        var result = await repo.GetByRequesterIdAsync("user-1");

        result.Should().BeEmpty();
    }
}
