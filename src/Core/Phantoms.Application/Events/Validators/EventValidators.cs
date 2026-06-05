using FluentValidation;
using Phantoms.Application.Events.Commands;

namespace Phantoms.Application.Events.Validators;

public class CreateEventCommandValidator : AbstractValidator<CreateEventCommand>
{
    public CreateEventCommandValidator()
    {
        RuleFor(x => x.Dto.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Dto.Description).MaximumLength(4000).When(x => x.Dto.Description is not null);
        RuleFor(x => x.Dto.Location).MaximumLength(250).When(x => x.Dto.Location is not null);
        RuleFor(x => x.Dto.StartsAt).NotEmpty();
        RuleFor(x => x.Dto.EndsAt)
            .GreaterThan(x => x.Dto.StartsAt)
            .When(x => x.Dto.EndsAt.HasValue);
    }
}

public class UpdateEventCommandValidator : AbstractValidator<UpdateEventCommand>
{
    public UpdateEventCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Dto.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Dto.Description).MaximumLength(4000).When(x => x.Dto.Description is not null);
        RuleFor(x => x.Dto.Location).MaximumLength(250).When(x => x.Dto.Location is not null);
        RuleFor(x => x.Dto.StartsAt).NotEmpty();
        RuleFor(x => x.Dto.EndsAt)
            .GreaterThan(x => x.Dto.StartsAt)
            .When(x => x.Dto.EndsAt.HasValue);
    }
}

public class DeleteEventCommandValidator : AbstractValidator<DeleteEventCommand>
{
    public DeleteEventCommandValidator() => RuleFor(x => x.Id).NotEmpty();
}

public class PublishEventCommandValidator : AbstractValidator<PublishEventCommand>
{
    public PublishEventCommandValidator() => RuleFor(x => x.Id).NotEmpty();
}

public class RejectEventCommandValidator : AbstractValidator<RejectEventCommand>
{
    public RejectEventCommandValidator() => RuleFor(x => x.Id).NotEmpty();
}
