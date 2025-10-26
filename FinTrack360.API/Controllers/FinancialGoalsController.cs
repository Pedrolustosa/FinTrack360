using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FinTrack360.Application.Features.FinancialGoals.CreateFinancialGoal;
using FinTrack360.Application.Features.FinancialGoals.GetFinancialGoalById;
using FinTrack360.Application.Features.FinancialGoals.GetFinancialGoals;
using FinTrack360.Application.Features.FinancialGoals.UpdateFinancialGoal;
using FinTrack360.Application.Features.FinancialGoals.DeleteFinancialGoal;
using FinTrack360.Application.Features.FinancialGoals.ContributeToFinancialGoal;

namespace FinTrack360.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FinancialGoalsController(ISender mediator) : ControllerBase
{
    private readonly ISender _mediator = mediator;

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateFinancialGoal([FromBody] CreateFinancialGoalCommand command)
    {
        var financialGoalId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetFinancialGoalById), new { id = financialGoalId }, new { id = financialGoalId });
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFinancialGoalById(Guid id)
    {
        var financialGoal = await _mediator.Send(new GetFinancialGoalByIdQuery(id));
        return financialGoal is not null ? Ok(financialGoal) : NotFound();
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFinancialGoals()
    {
        var financialGoals = await _mediator.Send(new GetFinancialGoalsQuery());
        return Ok(financialGoals);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateFinancialGoal(Guid id, [FromBody] UpdateFinancialGoalCommand command)
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
    public async Task<IActionResult> DeleteFinancialGoal(Guid id)
    {
        await _mediator.Send(new DeleteFinancialGoalCommand(id));
        return NoContent();
    }

    [HttpPost("{id:guid}/contribute")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ContributeToFinancialGoal(Guid id, [FromBody] ContributeToFinancialGoalDto dto)
    {
        var command = new ContributeToFinancialGoalCommand(id, dto.FromAccountId, dto.ToAccountId, dto.Amount);
        await _mediator.Send(command);
        return Ok(new { Message = "Contribution successful." });
    }
}
