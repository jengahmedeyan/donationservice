using Application.Donations.Queries.GetMyDonationRequests;
using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.Tests.Donations;

public class GetMyDonationRequestsHandlerTests
{
    private static DonationRequest OwnedBy(string owner, string item) => new()
    {
        RequesterId = owner,
        RequesterName = "Ada Lovelace",
        ItemNeeded = item,
        Location = "Toronto"
    };

    [Fact]
    public async Task Handle_ReturnsCallersOwnRequests_MappedToDtos()
    {
        var owned = new List<DonationRequest>
        {
            OwnedBy("user-1", "Winter coat"),
            OwnedBy("user-1", "Blankets")
        };
        var repo = new Mock<IDonationRepository>();
        repo.Setup(r => r.GetByRequesterIdAsync("user-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(owned);
        var handler = new GetMyDonationRequestsHandler(repo.Object);

        var result = await handler.Handle(
            new GetMyDonationRequestsQuery("user-1"), CancellationToken.None);

        result.Should().HaveCount(2);
        result.Select(d => d.ItemNeeded).Should().BeEquivalentTo("Winter coat", "Blankets");
    }

    [Fact]
    public async Task Handle_ScopesQueryToCaller_NeverWideningIt()
    {
        var repo = new Mock<IDonationRepository>();
        repo.Setup(r => r.GetByRequesterIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DonationRequest> { OwnedBy("user-1", "Winter coat") });
        var handler = new GetMyDonationRequestsHandler(repo.Object);

        var result = await handler.Handle(
            new GetMyDonationRequestsQuery("user-1"), CancellationToken.None);

        // The handler must ask the repo only for the caller's rows — never an unscoped fetch.
        repo.Verify(r => r.GetByRequesterIdAsync("user-1", It.IsAny<CancellationToken>()),
            Times.Once);
        repo.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
        result.Should().ContainSingle();
    }

    [Fact]
    public async Task Handle_ReturnsEmpty_WhenCallerHasNoRequests()
    {
        var repo = new Mock<IDonationRepository>();
        repo.Setup(r => r.GetByRequesterIdAsync("user-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DonationRequest>());
        var handler = new GetMyDonationRequestsHandler(repo.Object);

        var result = await handler.Handle(
            new GetMyDonationRequestsQuery("user-1"), CancellationToken.None);

        // Empty is a valid result — not null, no throw.
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}
