using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Phantoms.Application.Common.Interfaces;
using Phantoms.Application.Common.Models;
using Phantoms.Application.LostFounds.DTOs;
using Phantoms.Domain.Entities;

namespace Phantoms.Application.LostFounds.Commands;

public record CreateLostFoundCommand(CreateLostFoundDto Dto) : IRequest<Result<Guid>>;

public class CreateLostFoundCommandHandler(IAppDbContext context, IMapper mapper)
    : IRequestHandler<CreateLostFoundCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateLostFoundCommand request, CancellationToken cancellationToken)
    {
        var studentExists = await context.Students.AnyAsync(s => s.Id == request.Dto.StudentId, cancellationToken);
        if (!studentExists)
            return Result<Guid>.Failure("Student not found.");

        var item = mapper.Map<LostFound>(request.Dto);

        context.LostFounds.Add(item);
        await context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(item.Id, "Lost found post created successfully.");
    }
}

public record UpdateLostFoundCommand(Guid Id, UpdateLostFoundDto Dto) : IRequest<Result>;

public class UpdateLostFoundCommandHandler(IAppDbContext context, IMapper mapper)
    : IRequestHandler<UpdateLostFoundCommand, Result>
{
    public async Task<Result> Handle(UpdateLostFoundCommand request, CancellationToken cancellationToken)
    {
        var item = await context.LostFounds.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (item is null)
            return Result.Failure("Lost found post not found.");

        mapper.Map(request.Dto, item);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success("Lost found post updated successfully.");
    }
}

public record DeleteLostFoundCommand(Guid Id) : IRequest<Result>;

public class DeleteLostFoundCommandHandler(IAppDbContext context)
    : IRequestHandler<DeleteLostFoundCommand, Result>
{
    public async Task<Result> Handle(DeleteLostFoundCommand request, CancellationToken cancellationToken)
    {
        var item = await context.LostFounds.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (item is null)
            return Result.Failure("Lost found post not found.");

        item.IsDeleted = true;
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success("Lost found post deleted successfully.");
    }
}