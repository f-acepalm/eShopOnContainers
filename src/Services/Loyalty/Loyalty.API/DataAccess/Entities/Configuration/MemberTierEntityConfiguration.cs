using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Loyalty.API.DataAccess.Entities.Configuration;

public class MemberTierEntityConfiguration : IEntityTypeConfiguration<MemberTier>
{
    public void Configure(EntityTypeBuilder<MemberTier> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired();

        builder.Property(x => x.Discount)
           .IsRequired();

        builder.Property(x => x.Threshold)
           .IsRequired();
    }
}
