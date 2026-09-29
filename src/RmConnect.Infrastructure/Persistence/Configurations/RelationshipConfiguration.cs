using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RmConnect.Domain.Relationships;

namespace RmConnect.Infrastructure.Persistence.Configurations;

public class RelationshipConfiguration : IEntityTypeConfiguration<Relationship>
{
    public void Configure(EntityTypeBuilder<Relationship> builder)
    {
        builder.Property(r => r.EndReason).HasMaxLength(200);

        builder.HasOne(r => r.Customer).WithMany().HasForeignKey(r => r.CustomerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(r => r.Manager).WithMany().HasForeignKey(r => r.ManagerId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(r => r.CustomerId)
            .IsUnique()
            .HasFilter($"[Status] IN ('{RelationshipStatus.Pending}', '{RelationshipStatus.Active}')");
    }
}
