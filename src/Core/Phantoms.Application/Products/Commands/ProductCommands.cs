using MediatR;
using Phantoms.Application.Common.Interfaces;
using Phantoms.Application.Common.Models;
using Phantoms.Application.Products.DTOs;
using Phantoms.Domain.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Phantoms.Application.Products.Commands;

// ---- CREATE ----
public record CreateProductCommand(CreateProductDto Dto) : IRequest<Result<Guid>>;

public class CreateProductCommandHandler(IAppDbContext context, IMapper mapper)
    : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = mapper.Map<Product>(request.Dto);

        foreach (var (url, i) in request.Dto.ImageUrls.Select((u, i) => (u, i)))
            product.Images.Add(new ProductImage { Url = url, DisplayOrder = i });

        context.Products.Add(product);
        await context.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(product.Id, "Product created successfully.");
    }
}

// ---- UPDATE ----
public record UpdateProductCommand(Guid Id, UpdateProductDto Dto) : IRequest<Result>;

public class UpdateProductCommandHandler(IAppDbContext context, IMapper mapper)
    : IRequestHandler<UpdateProductCommand, Result>
{
    public async Task<Result> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await context.Products.FirstOrDefaultAsync(p => p.Id == request.Id && !p.IsDeleted, cancellationToken);
        if (product is null)
            return Result.Failure("Product not found.");

        mapper.Map(request.Dto, product);

        await context.SaveChangesAsync(cancellationToken);
        return Result.Success("Product updated successfully.");
    }
}

// ---- DELETE ----
public record DeleteProductCommand(Guid Id) : IRequest<Result>;

public class DeleteProductCommandHandler(IAppDbContext context)
    : IRequestHandler<DeleteProductCommand, Result>
{
    public async Task<Result> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await context.Products.FirstOrDefaultAsync(p => p.Id == request.Id && !p.IsDeleted, cancellationToken);
        if (product is null)
            return Result.Failure("Product not found.");

        product.IsDeleted = true;
        await context.SaveChangesAsync(cancellationToken);
        return Result.Success("Product deleted successfully.");
    }
}

