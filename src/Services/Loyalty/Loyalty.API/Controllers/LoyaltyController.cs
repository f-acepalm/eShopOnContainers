using Loyalty.API.DataAccess.Entities;
using Loyalty.API.DataAccess.Repositories;
using Loyalty.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Loyalty.API.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class LoyaltyController : ControllerBase
{
    private readonly ILoyaltyMemberRepository _loyaltyMemberrepository;
    private readonly IIdentityService _identityService;

    public LoyaltyController(
        ILoyaltyMemberRepository loyaltyMemberrepository,
        IIdentityService identityService)
    {
        _loyaltyMemberrepository = loyaltyMemberrepository;
        _identityService = identityService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LoyaltyMember>> GetLoyaltyMember()
    {
        var userId = Guid.Parse(_identityService.GetUserIdentity());
        LoyaltyMember? result = await _loyaltyMemberrepository.GetLoyaltyMemberAsync(userId);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }
}
