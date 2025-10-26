using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FinTrack360.Application.Features.Accounts.CreateAccount;
using FinTrack360.Application.Features.Accounts.GetAccountById;
using FinTrack360.Application.Features.Accounts.GetAccounts;
using FinTrack360.Application.Features.Accounts.UpdateAccount;
using FinTrack360.Application.Features.Accounts.DeleteAccount;
using FinTrack360.Application.Features.Transactions.ImportTransactions;

namespace FinTrack360.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AccountsController(ISender mediator) : ControllerBase
{
    private readonly ISender _mediator = mediator;

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountCommand command)
    {
        var accountId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetAccountById), new { id = accountId }, new { id = accountId });
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAccountById(Guid id)
    {
        var account = await _mediator.Send(new GetAccountByIdQuery(id));
        return account is not null ? Ok(account) : NotFound();
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAccounts()
    {
        var accounts = await _mediator.Send(new GetAccountsQuery());
        return Ok(accounts);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAccount(Guid id, [FromBody] UpdateAccountCommand command)
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
    public async Task<IActionResult> DeleteAccount(Guid id)
    {
        await _mediator.Send(new DeleteAccountCommand(id));
        return NoContent();
    }

    [HttpPost("{accountId:guid}/import")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ImportTransactions(Guid accountId, IFormFile file)
    {
        var command = new ImportTransactionsCommand(accountId, file);
        await _mediator.Send(command);
        return Ok(new { Message = "Transactions imported successfully." });
    }
}
