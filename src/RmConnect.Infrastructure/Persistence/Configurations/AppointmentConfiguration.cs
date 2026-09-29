using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RmConnect.Domain.Appointments;

namespace RmConnect.Infrastructure.Persistence.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.Property(a => a.Reason).HasMaxLength(200).IsRequired();
        builder.Property(a => a.CancellationReason).HasMaxLength(200);

        builder.HasOne(a => a.Customer).WithMany().HasForeignKey(a => a.CustomerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(a => a.Manager).WithMany().HasForeignKey(a => a.ManagerId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(a => new { a.ManagerId, a.StartsAt })
            .IsUnique()
            .HasFilter($"[Status] = '{AppointmentStatus.Booked}'");
    }
}
