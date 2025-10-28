using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FinTrack360.Application.Features.DebtPayoffPlanner;

namespace FinTrack360.API.Controllers;

[ApiController]
[Route("api/debt-payoff-planner")]
[Authorize]
public class DebtPayoffPlannerController(ISender mediator) : ControllerBase
{
    private string GetUserIdOrThrow()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? 
               throw new UnauthorizedAccessException("User ID not found in token.");
    }

    [HttpGet("snowball")]
    [ProducesResponseType(typeof(DebtPayoffPlanDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSnowballPlan([FromQuery] decimal extraPayment = 0)
    {
        var query = new GetSnowballPlanQuery(GetUserIdOrThrow(), extraPayment);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("avalanche")]
    [ProducesResponseType(typeof(DebtPayoffPlanDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAvalanchePlan([FromQuery] decimal extraPayment = 0)
    {
        var query = new GetAvalanchePlanQuery(GetUserIdOrThrow(), extraPayment);
        var result = await mediator.Send(query);
        return Ok(result);
    }
}
