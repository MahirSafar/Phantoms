using Phantoms.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Phantoms.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<Product> Products { get; }
    DbSet<ProductImage> ProductImages { get; }
    DbSet<Event> Events { get; }
    DbSet<Announcement> Announcements { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
