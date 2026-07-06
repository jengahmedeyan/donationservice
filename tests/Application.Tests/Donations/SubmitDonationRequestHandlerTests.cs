using Application.Donations.Commands.SubmitDonationRequest;
using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.Tests.Donations;

public class SubmitDonationRequestHandlerTests
{
    [Fact]
    public async Task Handle_PersistsRequest_AndReturnsMappedDto()
    {
        var repo = new Mock<IDonationRepository>();
        repo.Setup(r => r.AddAsync(It.IsAny<DonationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((DonationRequest d, CancellationToken _) => d);

        var handler = new SubmitDonationRequestHandler(repo.Object);
        var command = new SubmitDonationRequestCommand(
            RequesterId: "user-1",
            RequesterName: "Ada Lovelace",
            ItemNeeded: "Winter coat",
            Location: "Toronto");

        var result = await handler.Handle(command, CancellationToken.None);

        result.ItemNeeded.Should().Be("Winter coat");
        result.RequesterName.Should().Be("Ada Lovelace");
        result.Status.Should().Be("Open");
        repo.Verify(r => r.AddAsync(It.IsAny<DonationRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
