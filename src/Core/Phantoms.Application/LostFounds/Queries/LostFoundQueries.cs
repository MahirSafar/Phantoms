using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Phantoms.Application.Common.Interfaces;
using Phantoms.Application.Common.Models;
using Phantoms.Application.LostFounds.DTOs;

namespace Phantoms.Application.LostFounds.Queries;

public record GetLostFoundsQuery(int Page = 1, int PageSize = 10)
    : IRequest<Result<PaginatedResult<LostFoundDto>>>;

public class GetLostFoundsQueryHandler(IAppDbContext context, IMapper mapper)
    : IRequestHandler<GetLostFoundsQuery, Result<PaginatedResult<LostFoundDto>>>
{
    public async Task<Result<PaginatedResult<LostFoundDto>>> Handle(GetLostFoundsQuery request, CancellationToken cancellationToken)
    {
        var query = context.LostFounds.OrderByDescending(x => x.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<LostFoundDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return Result<PaginatedResult<LostFoundDto>>.Success(new PaginatedResult<LostFoundDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        });
    }
}

public record GetLostFoundByIdQuery(Guid Id) : IRequest<Result<LostFoundDto>>;

public class GetLostFoundByIdQueryHandler(IAppDbContext context, IMapper mapper)
    : IRequestHandler<GetLostFoundByIdQuery, Result<LostFoundDto>>
{
    public async Task<Result<LostFoundDto>> Handle(GetLostFoundByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await context.LostFounds
            .Where(x => x.Id == request.Id)
            .ProjectTo<LostFoundDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        if (item is null)
            return Result<LostFoundDto>.Failure("Lost found post not found.");

        return Result<LostFoundDto>.Success(item);
    }
}