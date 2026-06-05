using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Phantoms.Application.Common.Interfaces;
using Phantoms.Application.Common.Models;
using Phantoms.Application.TeamFinders.DTOs;
using Phantoms.Domain.Entities;

namespace Phantoms.Application.TeamFinders.Commands;

public record CreateTeamFinderCommand(CreateTeamFinderDto Dto) : IRequest<Result<Guid>>;

public class CreateTeamFinderCommandHandler(IAppDbContext context, IMapper mapper)
    : IRequestHandler<CreateTeamFinderCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateTeamFinderCommand request, CancellationToken cancellationToken)
    {
        var studentExists = await context.Students.AnyAsync(s => s.Id == request.Dto.StudentId, cancellationToken);
        if (!studentExists) return Result<Guid>.Failure("Student not found.");

        var item = mapper.Map<TeamFinder>(request.Dto);
        context.TeamFinders.Add(item);
        await context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(item.Id, "Team finder post created successfully.");
    }
}

public record UpdateTeamFinderCommand(Guid Id, UpdateTeamFinderDto Dto) : IRequest<Result>;

public class UpdateTeamFinderCommandHandler(IAppDbContext context, IMapper mapper)
    : IRequestHandler<UpdateTeamFinderCommand, Result>
{
    public async Task<Result> Handle(UpdateTeamFinderCommand request, CancellationToken cancellationToken)
    {
        var item = await context.TeamFinders.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (item is null) return Result.Failure("Team finder post not found.");

        mapper.Map(request.Dto, item);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success("Team finder post updated successfully.");
    }
}

public record DeleteTeamFinderCommand(Guid Id) : IRequest<Result>;

public class DeleteTeamFinderCommandHandler(IAppDbContext context)
    : IRequestHandler<DeleteTeamFinderCommand, Result>
{
    public async Task<Result> Handle(DeleteTeamFinderCommand request, CancellationToken cancellationToken)
    {
        var item = await context.TeamFinders.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (item is null) return Result.Failure("Team finder post not found.");

        item.IsDeleted = true;
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success("Team finder post deleted successfully.");
    }
}