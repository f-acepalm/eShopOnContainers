using Loyalty.API.DataAccess;
using Loyalty.API.DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Loyalty.API.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class LoyaltyController : ControllerBase
{
    private readonly ILoyaltyRepository _repository;

    public LoyaltyController(ILoyaltyRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<MemberTier>>> GetMemberTiers()
    {
        IEnumerable<MemberTier> result = await _repository.GetMemberTiers();

        if (!result.Any())
        {
            return NotFound();
        }

        return Ok(result);
    }
}
