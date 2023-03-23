using Loyalty.API.DataAccess.Entities;
using Loyalty.API.DataAccess.Repositories;
using Loyalty.API.Services;
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
    private readonly IIdentityService _identityService;

    public MemberTierController(
        IMemberTierRepository memberTierRepository,
        IIdentityService identityService)
    {
        _memberTierRepository = memberTierRepository;
        _identityService = identityService;
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

    [HttpGet("current")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<MemberTier>>> GetMemberTier()
    {
        var userId = Guid.Parse(_identityService.GetUserIdentity());
        var result = await _memberTierRepository.GetMemberTierAsync(userId);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }
}
