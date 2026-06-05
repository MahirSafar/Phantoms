using Phantoms.Application.Announcements.DTOs;
using Phantoms.Application.Common.Models;

namespace Phantoms.Application.Common.Interfaces;

public interface IAnnouncementServices
{
    Task<Result<Guid>> CreateAsync(CreateAnnouncementDto dto, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(Guid id, UpdateAnnouncementDto dto, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result> PublishAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result> RejectAsync(Guid id, CancellationToken cancellationToken = default);
}
