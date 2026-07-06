using Application.Common.Exceptions;
using Application.Donations.Queries.GetDonationRequestById;
using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.Tests.Donations;

public class GetDonationRequestByIdHandlerTests
{
    private static DonationRequest ExistingOwnedBy(string owner) => new()
    {
        RequesterId = owner,
        RequesterName = "Ada Lovelace",
        ItemNeeded = "Winter coat",
        Location = "Toronto"
    };

    [Fact]
    public async Task Handle_ReturnsDto_WhenCallerOwnsTheRequest()
    {
        var entity = ExistingOwnedBy("user-1");
        var repo = new Mock<IDonationRepository>();
        repo.Setup(r => r.GetByIdAsync(entity.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        var handler = new GetDonationRequestByIdHandler(repo.Object);

        var result = await handler.Handle(
            new GetDonationRequestByIdQuery(entity.Id, "user-1"), CancellationToken.None);

        result.Should().NotBeNull();
        result!.ItemNeeded.Should().Be("Winter coat");
    }

    [Fact]
    public async Task Handle_ReturnsNull_WhenNotFound()
    {
        var repo = new Mock<IDonationRepository>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((DonationRequest?)null);
        var handler = new GetDonationRequestByIdHandler(repo.Object);

        var result = await handler.Handle(
            new GetDonationRequestByIdQuery(Guid.NewGuid(), "user-1"), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_Throws_WhenCallerIsNotTheOwner()
    {
        var entity = ExistingOwnedBy("owner-1");
        var repo = new Mock<IDonationRepository>();
        repo.Setup(r => r.GetByIdAsync(entity.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        var handler = new GetDonationRequestByIdHandler(repo.Object);

        var act = async () => await handler.Handle(
            new GetDonationRequestByIdQuery(entity.Id, "attacker-2"), CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenAccessException>();
    }
}
