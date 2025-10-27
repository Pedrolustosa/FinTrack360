using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FinTrack360.Application.Features.Dashboard.KPIs.NetWorth;
using FinTrack360.Application.Features.Dashboard.KPIs.MonthlyCashFlow;

namespace FinTrack360.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController(ISender mediator) : ControllerBase
{
    private string GetUserIdOrThrow()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? 
               throw new UnauthorizedAccessException("User ID not found in token.");
    }

    [HttpGet("kpi/net-worth")]
    [ProducesResponseType(typeof(NetWorthDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetNetWorth()
    {
        var query = new GetNetWorthQuery(GetUserIdOrThrow());
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("kpi/monthly-cash-flow")]
    [ProducesResponseType(typeof(CashFlowDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMonthlyCashFlow()
    {
        var query = new GetMonthlyCashFlowQuery(GetUserIdOrThrow());
        var result = await mediator.Send(query);
        return Ok(result);
    }
}
