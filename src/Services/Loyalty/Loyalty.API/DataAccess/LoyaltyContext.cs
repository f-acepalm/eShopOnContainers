using Loyalty.API.DataAccess.Entities;
using Loyalty.API.DataAccess.Entities.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Loyalty.API.DataAccess;

public class LoyaltyContext : DbContext
{
	public LoyaltyContext(DbContextOptions<LoyaltyContext> options) : base(options) { }

    public DbSet<LoyaltyMember> LoyaltyMembers { get; set; }

    public DbSet<MemberTier> MemberTiers { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new LoyaltyMemberEntityConfiguration());
        builder.ApplyConfiguration(new MemberTierEntityConfiguration());
    }
}
