using Loyalty.API.DataAccess.Entities;
using Loyalty.API.DataAccess.Entities.Configuration;
using Loyalty.API.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Loyalty.API.DataAccess;

public class LoyaltyContext : DbContext, IUnitOfWork
{
	public LoyaltyContext(DbContextOptions<LoyaltyContext> options) : base(options) { }

    public DbSet<LoyaltyMember> LoyaltyMembers => Set<LoyaltyMember>();

    public DbSet<MemberTier> MemberTiers => Set<MemberTier>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new LoyaltyMemberEntityConfiguration());
        builder.ApplyConfiguration(new MemberTierEntityConfiguration());
    }
}
