using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Phantoms.Application.Common.Interfaces;
using Phantoms.Application.Common.Models;
using Phantoms.Application.TeamFinders.DTOs;

namespace Phantoms.Application.TeamFinders.Queries;

public record GetTeamFindersQuery(int Page = 1, int PageSize = 10)
    : IRequest<Result<PaginatedResult<TeamFinderDto>>>;

public class GetTeamFindersQueryHandler(IAppDbContext context, IMapper mapper)
    : IRequestHandler<GetTeamFindersQuery, Result<PaginatedResult<TeamFinderDto>>>
{
    public async Task<Result<PaginatedResult<TeamFinderDto>>> Handle(GetTeamFindersQuery request, CancellationToken cancellationToken)
    {
        var query = context.TeamFinders
            .Where(x => x.IsActive)
            .OrderByDescending(x => x.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<TeamFinderDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return Result<PaginatedResult<TeamFinderDto>>.Success(new PaginatedResult<TeamFinderDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        });
    }
}

public record GetTeamFinderByIdQuery(Guid Id) : IRequest<Result<TeamFinderDto>>;

public class GetTeamFinderByIdQueryHandler(IAppDbContext context, IMapper mapper)
    : IRequestHandler<GetTeamFinderByIdQuery, Result<TeamFinderDto>>
{
    public async Task<Result<TeamFinderDto>> Handle(GetTeamFinderByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await context.TeamFinders
            .Where(x => x.Id == request.Id)
            .ProjectTo<TeamFinderDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        if (item is null) return Result<TeamFinderDto>.Failure("Team finder post not found.");

        return Result<TeamFinderDto>.Success(item);
    }
}