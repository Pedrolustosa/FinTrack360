using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FinTrack360.Application.Features.Assets.AddAsset;
using FinTrack360.Application.Features.Assets.GetAssetById;
using FinTrack360.Application.Features.Assets.GetAssets;
using FinTrack360.Application.Features.Assets.UpdateAsset;
using FinTrack360.Application.Features.Assets.DeleteAsset;

namespace FinTrack360.API.Controllers;

[ApiController]
[Route("api/accounts/{accountId}/assets")]
[Authorize]
public class AssetsController(ISender mediator) : ControllerBase
{
    private readonly ISender _mediator = mediator;

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> AddAsset(Guid accountId, [FromBody] AddAssetDto dto)
    {
        var command = new AddAssetCommand(accountId, dto.Ticker, dto.Name, dto.Quantity, dto.AverageCost);
        var assetId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetAssetById), new { accountId, id = assetId }, new { id = assetId });
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAssetById(Guid accountId, Guid id)
    {
        var asset = await _mediator.Send(new GetAssetByIdQuery(accountId, id));
        return asset is not null ? Ok(asset) : NotFound();
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAssets(Guid accountId)
    {
        var assets = await _mediator.Send(new GetAssetsQuery(accountId));
        return Ok(assets);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsset(Guid accountId, Guid id, [FromBody] UpdateAssetDto dto)
    {
        var command = new UpdateAssetCommand(accountId, id, dto.Ticker, dto.Name, dto.Quantity, dto.AverageCost);
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsset(Guid accountId, Guid id)
    {
        await _mediator.Send(new DeleteAssetCommand(accountId, id));
        return NoContent();
    }
}
