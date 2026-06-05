using FluentValidation;
using Phantoms.Application.Announcements.Commands;

namespace Phantoms.Application.Announcements.Validators;

public class CreateAnnouncementCommandValidator : AbstractValidator<CreateAnnouncementCommand>
{
    public CreateAnnouncementCommandValidator()
    {
        RuleFor(x => x.Dto.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Dto.Content).NotEmpty().MaximumLength(4000);
        RuleFor(x => x.Dto.Category).IsInEnum();
    }
}

public class UpdateAnnouncementCommandValidator : AbstractValidator<UpdateAnnouncementCommand>
{
    public UpdateAnnouncementCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Dto.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Dto.Content).NotEmpty().MaximumLength(4000);
        RuleFor(x => x.Dto.Category).IsInEnum();
    }
}

public class DeleteAnnouncementCommandValidator : AbstractValidator<DeleteAnnouncementCommand>
{
    public DeleteAnnouncementCommandValidator() => RuleFor(x => x.Id).NotEmpty();
}

public class PublishAnnouncementCommandValidator : AbstractValidator<PublishAnnouncementCommand>
{
    public PublishAnnouncementCommandValidator() => RuleFor(x => x.Id).NotEmpty();
}

public class RejectAnnouncementCommandValidator : AbstractValidator<RejectAnnouncementCommand>
{
    public RejectAnnouncementCommandValidator() => RuleFor(x => x.Id).NotEmpty();
}
