using FinTrack360.Application.Common.Interfaces;
using FinTrack360.Domain.Entities;
using FinTrack360.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FinTrack360.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CategorizationRulesController : ControllerBase
{
    private readonly IAppDbContext _context;

    public CategorizationRulesController(IAppDbContext context)
    {
        _context = context;
    }

    private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException("User not found.");

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategorizationRule>>> GetRules()
    {
        var userId = GetUserId();
        var rules = await _context.CategorizationRules
            .Where(r => r.UserId == userId)
            .ToListAsync();
        return Ok(rules);
    }

    [HttpPost]
    public async Task<ActionResult<CategorizationRule>> CreateRule(CreateRuleDto dto)
    {
        var userId = GetUserId();

        // Verify category belongs to user
        var categoryExists = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId && c.UserId == userId);
        if (!categoryExists)
        {
            return BadRequest("Invalid CategoryId.");
        }

        var rule = new CategorizationRule
        {
            UserId = userId,
            RuleText = dto.RuleText,
            RuleType = dto.RuleType,
            CategoryId = dto.CategoryId
        };

        _context.CategorizationRules.Add(rule);
        await _context.SaveChangesAsync(default);

        return CreatedAtAction(nameof(GetRules), new { id = rule.Id }, rule);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRule(Guid id, UpdateRuleDto dto)
    {
        var userId = GetUserId();
        var rule = await _context.CategorizationRules.FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

        if (rule == null)
        {
            return NotFound();
        }

        // Verify category belongs to user
        var categoryExists = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId && c.UserId == userId);
        if (!categoryExists)
        {
            return BadRequest("Invalid CategoryId.");
        }

        rule.RuleText = dto.RuleText;
        rule.RuleType = dto.RuleType;
        rule.CategoryId = dto.CategoryId;

        await _context.SaveChangesAsync(default);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRule(Guid id)
    {
        var userId = GetUserId();
        var rule = await _context.CategorizationRules.FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

        if (rule == null)
        {
            return NotFound();
        }

        _context.CategorizationRules.Remove(rule);
        await _context.SaveChangesAsync(default);

        return NoContent();
    }
}

// DTOs for creation and update
public record CreateRuleDto(string RuleText, RuleType RuleType, Guid CategoryId);
public record UpdateRuleDto(string RuleText, RuleType RuleType, Guid CategoryId);
