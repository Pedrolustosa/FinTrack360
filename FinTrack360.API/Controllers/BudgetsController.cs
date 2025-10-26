using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FinTrack360.Application.Features.Budgets.CreateBudget;
using FinTrack360.Application.Features.Budgets.GetBudgetById;
using FinTrack360.Application.Features.Budgets.GetBudgets;
using FinTrack360.Application.Features.Budgets.UpdateBudget;
using FinTrack360.Application.Features.Budgets.DeleteBudget;

namespace FinTrack360.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BudgetsController(ISender mediator) : ControllerBase
{
    private readonly ISender _mediator = mediator;

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateBudget([FromBody] CreateBudgetCommand command)
    {
        var budgetId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetBudgetById), new { id = budgetId }, new { id = budgetId });
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBudgetById(Guid id)
    {
        var budget = await _mediator.Send(new GetBudgetByIdQuery(id));
        return budget is not null ? Ok(budget) : NotFound();
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBudgets([FromQuery] int? month, [FromQuery] int? year)
    {
        var budgets = await _mediator.Send(new GetBudgetsQuery(month, year));
        return Ok(budgets);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateBudget(Guid id, [FromBody] UpdateBudgetCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }

        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteBudget(Guid id)
    {
        await _mediator.Send(new DeleteBudgetCommand(id));
        return NoContent();
    }
}
