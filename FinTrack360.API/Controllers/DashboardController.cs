using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FinTrack360.Application.Features.Dashboard.KPIs.NetWorth;
using FinTrack360.Application.Features.Dashboard.KPIs.MonthlyCashFlow;
using FinTrack360.Application.Features.Dashboard.BudgetSummary;
using FinTrack360.Application.Features.Dashboard.SpendingByCategoryChart;
using FinTrack360.Application.Features.Dashboard.UpcomingBills;
using FinTrack360.Application.Features.Dashboard.AccountSummary;
using FinTrack360.Application.Features.Dashboard.RecentTransactions;

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

    [HttpGet("budget-summary")]
    [ProducesResponseType(typeof(List<BudgetReportDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetBudgetSummary()
    {
        var query = new GetBudgetSummaryQuery(GetUserIdOrThrow());
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("spending-by-category-chart")]
    [ProducesResponseType(typeof(List<CategorySpendingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetSpendingByCategoryChart()
    {
        var query = new GetSpendingByCategoryChartQuery(GetUserIdOrThrow());
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("upcoming-bills")]
    [ProducesResponseType(typeof(List<UpcomingBillDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUpcomingBills()
    {
        var query = new GetUpcomingBillsQuery(GetUserIdOrThrow());
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("account-summary")]
    [ProducesResponseType(typeof(List<AccountSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAccountSummary()
    {
        var query = new GetAccountSummaryQuery(GetUserIdOrThrow());
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("recent-transactions")]
    [ProducesResponseType(typeof(List<RecentTransactionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetRecentTransactions()
    {
        var query = new GetRecentTransactionsQuery(GetUserIdOrThrow());
        var result = await mediator.Send(query);
        return Ok(result);
    }
}
