using Microsoft.EntityFrameworkCore;
using RmConnect.Domain.Appointments;
using RmConnect.Domain.Relationships;
using RmConnect.Domain.Users;

namespace RmConnect.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<User> Users { get; }
    DbSet<UserSession> Sessions { get; }
    DbSet<Relationship> Relationships { get; }
    DbSet<Appointment> Appointments { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
