using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Phantoms.Application.Common.Interfaces;
using Phantoms.Application.Common.Models;
using Phantoms.Application.Events.DTOs;
using Phantoms.Domain.Constants;
using Phantoms.Domain.Enums;

namespace Phantoms.Application.Events.Queries;

public record GetEventsQuery(
    int Page = 1,
    int PageSize = 10,
    PublishStatus? Status = null,
    bool ApplyCurrentTeacherScope = true) : IRequest<Result<PaginatedResult<EventDto>>>;

public class GetEventsQueryHandler(IAppDbContext context, IMapper mapper, ICurrentUserService currentUserService)
    : IRequestHandler<GetEventsQuery, Result<PaginatedResult<EventDto>>>
{
    public async Task<Result<PaginatedResult<EventDto>>> Handle(GetEventsQuery request, CancellationToken cancellationToken)
    {
        var query = context.Events.AsQueryable();

        if (request.ApplyCurrentTeacherScope && currentUserService.IsInRole(Roles.Teacher))
        {
            if (!Guid.TryParse(currentUserService.UserId, out var teacherId))
                return Result<PaginatedResult<EventDto>>.Failure("Authenticated teacher id was not found.");

            query = query.Where(e => e.TeacherId == teacherId);
        }
        else
        {
            query = query.Where(e => e.Status == (request.Status ?? PublishStatus.Published));
        }

        if (request.Status.HasValue && (!request.ApplyCurrentTeacherScope || !currentUserService.IsInRole(Roles.Teacher)))
            query = query.Where(e => e.Status == request.Status.Value);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(e => e.StartsAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<EventDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return Result<PaginatedResult<EventDto>>.Success(new PaginatedResult<EventDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        });
    }
}

public record GetEventByIdQuery(Guid Id) : IRequest<Result<EventDto>>;

public class GetEventByIdQueryHandler(IAppDbContext context, IMapper mapper, ICurrentUserService currentUserService)
    : IRequestHandler<GetEventByIdQuery, Result<EventDto>>
{
    public async Task<Result<EventDto>> Handle(GetEventByIdQuery request, CancellationToken cancellationToken)
    {
        var query = context.Events.Where(e => e.Id == request.Id);

        if (currentUserService.IsInRole(Roles.Teacher))
        {
            if (!Guid.TryParse(currentUserService.UserId, out var teacherId))
                return Result<EventDto>.Failure("Authenticated teacher id was not found.");

            query = query.Where(e => e.TeacherId == teacherId);
        }
        else if (!currentUserService.IsInRole(Roles.Admin))
        {
            query = query.Where(e => e.Status == PublishStatus.Published);
        }

        var entity = await query.ProjectTo<EventDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken);

        return entity is null
            ? Result<EventDto>.Failure("Event not found.")
            : Result<EventDto>.Success(entity);
    }
}
