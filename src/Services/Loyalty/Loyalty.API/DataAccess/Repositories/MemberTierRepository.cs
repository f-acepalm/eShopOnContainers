using Loyalty.API.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Loyalty.API.DataAccess.Repositories;

public class MemberTierRepository : IMemberTierRepository
{
    private readonly LoyaltyContext _context;

    public IUnitOfWork UnitOfWork => _context;

    public MemberTierRepository(LoyaltyContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<MemberTier>> GetMemberTiersAsync()
    {
        return await _context.MemberTiers.ToListAsync();
    }

    public async Task<MemberTier?> GetMemberTierAsync(Guid userId)
    {
        return await _context.MemberTiers
            .OrderByDescending(tier => tier.Threshold)
            .FirstOrDefaultAsync(tier => tier.Threshold <= _context.LoyaltyMembers.First(member => member.Id == userId).TotalSpent);
    }
}
