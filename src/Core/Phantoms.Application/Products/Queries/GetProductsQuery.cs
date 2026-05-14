using MediatR;
using Phantoms.Application.Common.Interfaces;
using Phantoms.Application.Common.Models;
using Phantoms.Application.Products.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Phantoms.Application.Products.Queries;

public record GetProductsQuery(int Page = 1, int PageSize = 10, string? Category = null) : IRequest<Result<PaginatedResult<ProductDto>>>;

public class GetProductsQueryHandler(IAppDbContext context, IMapper mapper)
    : IRequestHandler<GetProductsQuery, Result<PaginatedResult<ProductDto>>>
{
    public async Task<Result<PaginatedResult<ProductDto>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var query = context.Products
            .Include(p => p.Images)
            .Where(p => !p.IsDeleted && p.IsActive);

        if (!string.IsNullOrWhiteSpace(request.Category))
            query = query.Where(p => p.Category == request.Category);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<ProductDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return Result<PaginatedResult<ProductDto>>.Success(new PaginatedResult<ProductDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        });
    }
}

public record GetProductByIdQuery(Guid Id) : IRequest<Result<ProductDto>>;

public class GetProductByIdQueryHandler(IAppDbContext context, IMapper mapper)
    : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
{
    public async Task<Result<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await context.Products
            .Include(p => p.Images)
            .Where(p => !p.IsDeleted && p.Id == request.Id)
            .ProjectTo<ProductDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        if (product is null)
            return Result<ProductDto>.Failure("Product not found.");

        return Result<ProductDto>.Success(product);
    }
}
