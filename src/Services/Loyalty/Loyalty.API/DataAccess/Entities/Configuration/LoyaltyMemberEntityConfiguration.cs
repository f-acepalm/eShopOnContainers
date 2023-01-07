using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Loyalty.API.DataAccess.Entities.Configuration;

public class LoyaltyMemberEntityConfiguration : IEntityTypeConfiguration<LoyaltyMember>
{
    public void Configure(EntityTypeBuilder<LoyaltyMember> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.TotalSpent)
            .IsRequired();

        builder.Property(x => x.Points)
           .IsRequired();
    }
}
