using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FinTrack360.Application.Features.Reports.GetSpendingByCategory;
using FinTrack360.Application.Features.Reports.GetCashFlow;
using FinTrack360.Application.Features.Reports.GetNetWorth;
using System;

namespace FinTrack360.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController(ISender mediator) : ControllerBase
{
    private readonly ISender _mediator = mediator;

    [HttpGet("spending-by-category")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSpendingByCategory([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var query = new GetSpendingByCategoryQuery(startDate, endDate);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("cash-flow")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCashFlow([FromQuery] int year)
    {
        var query = new GetCashFlowQuery(year);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("net-worth")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetNetWorth()
    {
        var query = new GetNetWorthQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
