using Application.Donations.Commands.CancelDonationRequest;
using Application.Donations.Commands.SubmitDonationRequest;
using Application.Donations.Dtos;
using Application.Donations.Queries.GetDonationRequestById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/donations")]
public class DonationsController : ControllerBase
{
    private readonly ISender _sender;

    public DonationsController(ISender sender) => _sender = sender;

    [HttpPost]
    public async Task<ActionResult<DonationRequestDto>> Submit(
        [FromBody] SubmitDonationRequestCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        // Stands in for the authenticated subject (real app: User.FindFirstValue("sub")).
        var requesterId = Request.Headers["X-Requester-Id"].ToString();
        if (string.IsNullOrWhiteSpace(requesterId))
            return Unauthorized();

        var result = await _sender.Send(
            new GetDonationRequestByIdQuery(id, requesterId), cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        // Stands in for the authenticated subject (real app: User.FindFirstValue("sub")).
        var requesterId = Request.Headers["X-Requester-Id"].ToString();
        if (string.IsNullOrWhiteSpace(requesterId))
            return Unauthorized();

        var result = await _sender.Send(
            new CancelDonationRequestCommand(id, requesterId), cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }
}
