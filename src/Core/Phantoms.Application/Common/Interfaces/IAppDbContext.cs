using Phantoms.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Phantoms.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<Product> Products { get; }
    DbSet<ProductImage> ProductImages { get; }
<<<<<<< Updated upstream
    DbSet<Event> Events { get; }
    DbSet<Announcement> Announcements { get; }
=======
    DbSet<Student> Students { get; }
    DbSet<LostFound> LostFounds { get; }
    DbSet<TeamFinder> TeamFinders { get; }
>>>>>>> Stashed changes
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
