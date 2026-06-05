using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Phantoms.Application.Common.Interfaces;
using Phantoms.Application.Common.Models;
using Phantoms.Application.Events.DTOs;
using Phantoms.Domain.Entities;
using Phantoms.Domain.Enums;

namespace Phantoms.Application.Events.Services;

public class EventServices(IAppDbContext context, ICurrentUserService currentUserService, IMapper mapper) : IEventServices
{
    public async Task<Result<Guid>> CreateAsync(CreateEventDto dto, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(currentUserService.UserId, out var teacherId))
            return Result<Guid>.Failure("Authenticated teacher id was not found.");

        var entity = mapper.Map<Event>(dto);
        entity.TeacherId = teacherId;
        entity.Status = PublishStatus.Pending;

        context.Events.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(entity.Id, "Event submitted for admin approval.");
    }

    public async Task<Result> UpdateAsync(Guid id, UpdateEventDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await context.Events.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        if (entity is null)
            return Result.Failure("Event not found.");

        if (!IsCurrentTeacher(entity.TeacherId))
            return Result.Failure("You can only update your own events.");

        if (entity.Status == PublishStatus.Published)
            return Result.Failure("Published events cannot be edited.");

        mapper.Map(dto, entity);
        entity.Status = PublishStatus.Pending;

        await context.SaveChangesAsync(cancellationToken);
        return Result.Success("Event updated successfully.");
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await context.Events.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        if (entity is null)
            return Result.Failure("Event not found.");

        if (!IsCurrentTeacher(entity.TeacherId))
            return Result.Failure("You can only delete your own events.");

        entity.IsDeleted = true;
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success("Event deleted successfully.");
    }

    public async Task<Result> PublishAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await context.Events.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        if (entity is null)
            return Result.Failure("Event not found.");

        entity.Status = PublishStatus.Published;
        entity.PublishedAt = DateTime.UtcNow;
        entity.PublishedBy = currentUserService.UserName ?? "admin";

        await context.SaveChangesAsync(cancellationToken);
        return Result.Success("Event published successfully.");
    }

    public async Task<Result> RejectAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await context.Events.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        if (entity is null)
            return Result.Failure("Event not found.");

        entity.Status = PublishStatus.Rejected;
        entity.PublishedAt = null;
        entity.PublishedBy = null;

        await context.SaveChangesAsync(cancellationToken);
        return Result.Success("Event rejected successfully.");
    }

    private bool IsCurrentTeacher(Guid teacherId) =>
        Guid.TryParse(currentUserService.UserId, out var currentUserId) && currentUserId == teacherId;
}
