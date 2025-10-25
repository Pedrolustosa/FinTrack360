using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FinTrack360.Application.Features.Profile.UpdateProfile;
using FinTrack360.Application.Features.Profile.DeleteAccount;

namespace FinTrack360.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController(ISender mediator) : ControllerBase
{
    private readonly ISender _mediator = mediator;

    [HttpPut("me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileCommand command)
    {
        await _mediator.Send(command);
        return Ok(new { Message = "Profile updated successfully." });
    }

    [HttpDelete("me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteAccount([FromBody] DeleteAccountCommand command)
    {
        await _mediator.Send(command);
        return Ok(new { Message = "Account deleted successfully." });
    }
}
