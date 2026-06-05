using MediatR;
using Phantoms.Application.Common.Interfaces;
using Phantoms.Application.Common.Models;
using Phantoms.Application.Events.DTOs;

namespace Phantoms.Application.Events.Commands;

public record CreateEventCommand(CreateEventDto Dto) : IRequest<Result<Guid>>;
public record UpdateEventCommand(Guid Id, UpdateEventDto Dto) : IRequest<Result>;
public record DeleteEventCommand(Guid Id) : IRequest<Result>;
public record PublishEventCommand(Guid Id) : IRequest<Result>;
public record RejectEventCommand(Guid Id) : IRequest<Result>;

public class CreateEventCommandHandler(IEventServices eventServices) : IRequestHandler<CreateEventCommand, Result<Guid>>
{
    public Task<Result<Guid>> Handle(CreateEventCommand request, CancellationToken cancellationToken) =>
        eventServices.CreateAsync(request.Dto, cancellationToken);
}

public class UpdateEventCommandHandler(IEventServices eventServices) : IRequestHandler<UpdateEventCommand, Result>
{
    public Task<Result> Handle(UpdateEventCommand request, CancellationToken cancellationToken) =>
        eventServices.UpdateAsync(request.Id, request.Dto, cancellationToken);
}

public class DeleteEventCommandHandler(IEventServices eventServices) : IRequestHandler<DeleteEventCommand, Result>
{
    public Task<Result> Handle(DeleteEventCommand request, CancellationToken cancellationToken) =>
        eventServices.DeleteAsync(request.Id, cancellationToken);
}

public class PublishEventCommandHandler(IEventServices eventServices) : IRequestHandler<PublishEventCommand, Result>
{
    public Task<Result> Handle(PublishEventCommand request, CancellationToken cancellationToken) =>
        eventServices.PublishAsync(request.Id, cancellationToken);
}

public class RejectEventCommandHandler(IEventServices eventServices) : IRequestHandler<RejectEventCommand, Result>
{
    public Task<Result> Handle(RejectEventCommand request, CancellationToken cancellationToken) =>
        eventServices.RejectAsync(request.Id, cancellationToken);
}
