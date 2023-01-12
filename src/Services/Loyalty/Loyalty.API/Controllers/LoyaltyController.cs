using Loyalty.API.DataAccess.Entities;
using Loyalty.API.DataAccess.Repositories;
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

    public LoyaltyController(ILoyaltyMemberRepository loyaltyMemberrepository)
    {
        _loyaltyMemberrepository = loyaltyMemberrepository;
    }

    [HttpGet("{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LoyaltyMember>> GetLoyaltyMember(Guid userId)
    {
        LoyaltyMember? result = await _loyaltyMemberrepository.GetLoyaltyMemberAsync(userId);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }
}
