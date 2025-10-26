using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FinTrack360.Application.Features.RecurringTransactions.CreateRecurringTransaction;
using FinTrack360.Application.Features.RecurringTransactions.GetRecurringTransactionById;
using FinTrack360.Application.Features.RecurringTransactions.GetRecurringTransactions;
using FinTrack360.Application.Features.RecurringTransactions.UpdateRecurringTransaction;
using FinTrack360.Application.Features.RecurringTransactions.DeleteRecurringTransaction;

namespace FinTrack360.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RecurringTransactionsController(ISender mediator) : ControllerBase
{
    private readonly ISender _mediator = mediator;

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateRecurringTransaction([FromBody] CreateRecurringTransactionCommand command)
    {
        var recurringTransactionId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetRecurringTransactionById), new { id = recurringTransactionId }, new { id = recurringTransactionId });
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRecurringTransactionById(Guid id)
    {
        var recurringTransaction = await _mediator.Send(new GetRecurringTransactionByIdQuery(id));
        return recurringTransaction is not null ? Ok(recurringTransaction) : NotFound();
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRecurringTransactions()
    {
        var recurringTransactions = await _mediator.Send(new GetRecurringTransactionsQuery());
        return Ok(recurringTransactions);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateRecurringTransaction(Guid id, [FromBody] UpdateRecurringTransactionCommand command)
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
    public async Task<IActionResult> DeleteRecurringTransaction(Guid id)
    {
        await _mediator.Send(new DeleteRecurringTransactionCommand(id));
        return NoContent();
    }
}
