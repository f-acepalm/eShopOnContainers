using Loyalty.API.DataAccess.Entities;
using System;
using System.Threading.Tasks;

namespace Loyalty.API.DataAccess.Repositories;

public class LoyaltyRepository : ILoyaltyMemberRepository
{
    private readonly LoyaltyContext _context;

    public IUnitOfWork UnitOfWork => _context;

    public LoyaltyRepository(LoyaltyContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<LoyaltyMember?> GetLoyaltyMemberAsync(Guid userId)
    {
        return await _context.LoyaltyMembers.FindAsync(userId);
    }

    public void AddLoyaltyMember(LoyaltyMember entity)
    {
        _context.LoyaltyMembers.Add(entity);
    }

    public void UpdateLoyaltyMember(LoyaltyMember entity)
    {
        _context.LoyaltyMembers.Update(entity);
    }
}
