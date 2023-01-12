using Loyalty.API.DataAccess.Entities;
using Loyalty.API.DataAccess.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Loyalty.API.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class MemberTierController : ControllerBase
{
    private readonly IMemberTierRepository _memberTierRepository;

    public MemberTierController(IMemberTierRepository memberTierRepository)
    {
        _memberTierRepository = memberTierRepository;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<MemberTier>>> GetAllMemberTiers()
    {
        IEnumerable<MemberTier> result = await _memberTierRepository.GetMemberTiersAsync();

        if (!result.Any())
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpGet("{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<MemberTier>>> GetMemberTier(Guid userId)
    {
        var result = await _memberTierRepository.GetMemberTierAsync(userId);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }
}
