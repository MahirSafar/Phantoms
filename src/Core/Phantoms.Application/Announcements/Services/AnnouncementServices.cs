using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Phantoms.Application.Announcements.DTOs;
using Phantoms.Application.Common.Interfaces;
using Phantoms.Application.Common.Models;
using Phantoms.Domain.Entities;
using Phantoms.Domain.Enums;

namespace Phantoms.Application.Announcements.Services;

public class AnnouncementServices(IAppDbContext context, ICurrentUserService currentUserService, IMapper mapper) : IAnnouncementServices
{
    public async Task<Result<Guid>> CreateAsync(CreateAnnouncementDto dto, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(currentUserService.UserId, out var teacherId))
            return Result<Guid>.Failure("Authenticated teacher id was not found.");

        var entity = mapper.Map<Announcement>(dto);
        entity.TeacherId = teacherId;
        entity.Status = PublishStatus.Pending;

        context.Announcements.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(entity.Id, "Announcement submitted for admin approval.");
    }

    public async Task<Result> UpdateAsync(Guid id, UpdateAnnouncementDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await context.Announcements.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        if (entity is null)
            return Result.Failure("Announcement not found.");

        if (!IsCurrentTeacher(entity.TeacherId))
            return Result.Failure("You can only update your own announcements.");

        if (entity.Status == PublishStatus.Published)
            return Result.Failure("Published announcements cannot be edited.");

        mapper.Map(dto, entity);
        entity.Status = PublishStatus.Pending;

        await context.SaveChangesAsync(cancellationToken);
        return Result.Success("Announcement updated successfully.");
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await context.Announcements.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        if (entity is null)
            return Result.Failure("Announcement not found.");

        if (!IsCurrentTeacher(entity.TeacherId))
            return Result.Failure("You can only delete your own announcements.");

        entity.IsDeleted = true;
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success("Announcement deleted successfully.");
    }

    public async Task<Result> PublishAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await context.Announcements.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        if (entity is null)
            return Result.Failure("Announcement not found.");

        entity.Status = PublishStatus.Published;
        entity.PublishedAt = DateTime.UtcNow;
        entity.PublishedBy = currentUserService.UserName ?? "admin";

        await context.SaveChangesAsync(cancellationToken);
        return Result.Success("Announcement published successfully.");
    }

    public async Task<Result> RejectAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await context.Announcements.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        if (entity is null)
            return Result.Failure("Announcement not found.");

        entity.Status = PublishStatus.Rejected;
        entity.PublishedAt = null;
        entity.PublishedBy = null;

        await context.SaveChangesAsync(cancellationToken);
        return Result.Success("Announcement rejected successfully.");
    }

    private bool IsCurrentTeacher(Guid teacherId) =>
        Guid.TryParse(currentUserService.UserId, out var currentUserId) && currentUserId == teacherId;
}
