using Application.Common.Exceptions;
using Application.Donations.Commands.CancelDonationRequest;
using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.Tests.Donations;

public class CancelDonationRequestHandlerTests
{
    private static DonationRequest ExistingOwnedBy(string owner) => new()
    {
        RequesterId = owner,
        RequesterName = "Ada Lovelace",
        ItemNeeded = "Winter coat",
        Location = "Toronto"
    };

    [Fact]
    public async Task Handle_CancelsRequest_WhenCallerOwnsAnOpenRequest()
    {
        var entity = ExistingOwnedBy("user-1");
        var repo = new Mock<IDonationRepository>();
        repo.Setup(r => r.GetByIdAsync(entity.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        var handler = new CancelDonationRequestHandler(repo.Object);

        var result = await handler.Handle(
            new CancelDonationRequestCommand(entity.Id, "user-1"), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Status.Should().Be("Cancelled");
        repo.Verify(r => r.UpdateAsync(
            It.Is<DonationRequest>(d => d.Status == DonationStatus.Cancelled),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnsNull_WhenNotFound()
    {
        var repo = new Mock<IDonationRepository>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((DonationRequest?)null);
        var handler = new CancelDonationRequestHandler(repo.Object);

        var result = await handler.Handle(
            new CancelDonationRequestCommand(Guid.NewGuid(), "user-1"), CancellationToken.None);

        result.Should().BeNull();
        repo.Verify(r => r.UpdateAsync(It.IsAny<DonationRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Throws_WhenCallerIsNotTheOwner()
    {
        var entity = ExistingOwnedBy("owner-1");
        var repo = new Mock<IDonationRepository>();
        repo.Setup(r => r.GetByIdAsync(entity.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        var handler = new CancelDonationRequestHandler(repo.Object);

        var act = async () => await handler.Handle(
            new CancelDonationRequestCommand(entity.Id, "attacker-2"), CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenAccessException>();
        repo.Verify(r => r.UpdateAsync(It.IsAny<DonationRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Throws_WhenRequestIsAlreadyCancelled()
    {
        var entity = ExistingOwnedBy("user-1");
        entity.Cancel();
        var repo = new Mock<IDonationRepository>();
        repo.Setup(r => r.GetByIdAsync(entity.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        var handler = new CancelDonationRequestHandler(repo.Object);

        var act = async () => await handler.Handle(
            new CancelDonationRequestCommand(entity.Id, "user-1"), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidDonationStateException>();
        repo.Verify(r => r.UpdateAsync(It.IsAny<DonationRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
