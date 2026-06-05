using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Phantoms.Application.Announcements.DTOs;
using Phantoms.Application.Common.Interfaces;
using Phantoms.Application.Common.Models;
using Phantoms.Domain.Constants;
using Phantoms.Domain.Enums;

namespace Phantoms.Application.Announcements.Queries;

public record GetAnnouncementsQuery(
    int Page = 1,
    int PageSize = 10,
    PublishStatus? Status = null,
    AnnouncementCategory? Category = null,
    bool ApplyCurrentTeacherScope = true) : IRequest<Result<PaginatedResult<AnnouncementDto>>>;

public class GetAnnouncementsQueryHandler(IAppDbContext context, IMapper mapper, ICurrentUserService currentUserService)
    : IRequestHandler<GetAnnouncementsQuery, Result<PaginatedResult<AnnouncementDto>>>
{
    public async Task<Result<PaginatedResult<AnnouncementDto>>> Handle(GetAnnouncementsQuery request, CancellationToken cancellationToken)
    {
        var query = context.Announcements.AsQueryable();

        if (request.ApplyCurrentTeacherScope && currentUserService.IsInRole(Roles.Teacher))
        {
            if (!Guid.TryParse(currentUserService.UserId, out var teacherId))
                return Result<PaginatedResult<AnnouncementDto>>.Failure("Authenticated teacher id was not found.");

            query = query.Where(a => a.TeacherId == teacherId);
        }
        else
        {
            query = query.Where(a => a.Status == (request.Status ?? PublishStatus.Published));
        }

        if (request.Status.HasValue && (!request.ApplyCurrentTeacherScope || !currentUserService.IsInRole(Roles.Teacher)))
            query = query.Where(a => a.Status == request.Status.Value);

        if (request.Category.HasValue)
            query = query.Where(a => a.Category == request.Category.Value);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<AnnouncementDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return Result<PaginatedResult<AnnouncementDto>>.Success(new PaginatedResult<AnnouncementDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        });
    }
}

public record GetAnnouncementByIdQuery(Guid Id) : IRequest<Result<AnnouncementDto>>;

public class GetAnnouncementByIdQueryHandler(IAppDbContext context, IMapper mapper, ICurrentUserService currentUserService)
    : IRequestHandler<GetAnnouncementByIdQuery, Result<AnnouncementDto>>
{
    public async Task<Result<AnnouncementDto>> Handle(GetAnnouncementByIdQuery request, CancellationToken cancellationToken)
    {
        var query = context.Announcements.Where(a => a.Id == request.Id);

        if (currentUserService.IsInRole(Roles.Teacher))
        {
            if (!Guid.TryParse(currentUserService.UserId, out var teacherId))
                return Result<AnnouncementDto>.Failure("Authenticated teacher id was not found.");

            query = query.Where(a => a.TeacherId == teacherId);
        }
        else if (!currentUserService.IsInRole(Roles.Admin))
        {
            query = query.Where(a => a.Status == PublishStatus.Published);
        }

        var entity = await query.ProjectTo<AnnouncementDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken);

        return entity is null
            ? Result<AnnouncementDto>.Failure("Announcement not found.")
            : Result<AnnouncementDto>.Success(entity);
    }
}
