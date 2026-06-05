using Phantoms.Application.Common.Models;
using Phantoms.Application.Events.DTOs;

namespace Phantoms.Application.Common.Interfaces;

public interface IEventServices
{
    Task<Result<Guid>> CreateAsync(CreateEventDto dto, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(Guid id, UpdateEventDto dto, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result> PublishAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result> RejectAsync(Guid id, CancellationToken cancellationToken = default);
}
