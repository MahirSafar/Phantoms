using MediatR;
using Phantoms.Application.Announcements.DTOs;
using Phantoms.Application.Common.Interfaces;
using Phantoms.Application.Common.Models;

namespace Phantoms.Application.Announcements.Commands;

public record CreateAnnouncementCommand(CreateAnnouncementDto Dto) : IRequest<Result<Guid>>;
public record UpdateAnnouncementCommand(Guid Id, UpdateAnnouncementDto Dto) : IRequest<Result>;
public record DeleteAnnouncementCommand(Guid Id) : IRequest<Result>;
public record PublishAnnouncementCommand(Guid Id) : IRequest<Result>;
public record RejectAnnouncementCommand(Guid Id) : IRequest<Result>;

public class CreateAnnouncementCommandHandler(IAnnouncementServices announcementServices) : IRequestHandler<CreateAnnouncementCommand, Result<Guid>>
{
    public Task<Result<Guid>> Handle(CreateAnnouncementCommand request, CancellationToken cancellationToken) =>
        announcementServices.CreateAsync(request.Dto, cancellationToken);
}

public class UpdateAnnouncementCommandHandler(IAnnouncementServices announcementServices) : IRequestHandler<UpdateAnnouncementCommand, Result>
{
    public Task<Result> Handle(UpdateAnnouncementCommand request, CancellationToken cancellationToken) =>
        announcementServices.UpdateAsync(request.Id, request.Dto, cancellationToken);
}

public class DeleteAnnouncementCommandHandler(IAnnouncementServices announcementServices) : IRequestHandler<DeleteAnnouncementCommand, Result>
{
    public Task<Result> Handle(DeleteAnnouncementCommand request, CancellationToken cancellationToken) =>
        announcementServices.DeleteAsync(request.Id, cancellationToken);
}

public class PublishAnnouncementCommandHandler(IAnnouncementServices announcementServices) : IRequestHandler<PublishAnnouncementCommand, Result>
{
    public Task<Result> Handle(PublishAnnouncementCommand request, CancellationToken cancellationToken) =>
        announcementServices.PublishAsync(request.Id, cancellationToken);
}

public class RejectAnnouncementCommandHandler(IAnnouncementServices announcementServices) : IRequestHandler<RejectAnnouncementCommand, Result>
{
    public Task<Result> Handle(RejectAnnouncementCommand request, CancellationToken cancellationToken) =>
        announcementServices.RejectAsync(request.Id, cancellationToken);
}
