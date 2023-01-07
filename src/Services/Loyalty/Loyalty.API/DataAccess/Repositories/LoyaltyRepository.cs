using Loyalty.API.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Loyalty.API.DataAccess.Repositories;

public class LoyaltyRepository : ILoyaltyRepository
{
    private readonly LoyaltyContext _context;

    public LoyaltyRepository(LoyaltyContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<MemberTier>> GetMemberTiers()
    {
        return await _context.MemberTiers.ToListAsync();
    }
}
