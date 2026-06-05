using FluentValidation;
using Phantoms.Application.LostFounds.Commands;

namespace Phantoms.Application.LostFounds.Validators;

public class CreateLostFoundCommandValidator : AbstractValidator<CreateLostFoundCommand>
{
    public CreateLostFoundCommandValidator()
    {
        RuleFor(x => x.Dto.StudentId).NotEmpty();
        RuleFor(x => x.Dto.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Dto.Description).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Dto.Location).MaximumLength(300);
        RuleFor(x => x.Dto.ImageUrl).MaximumLength(500);
    }
}

public class UpdateLostFoundCommandValidator : AbstractValidator<UpdateLostFoundCommand>
{
    public UpdateLostFoundCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Dto.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Dto.Description).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Dto.Location).MaximumLength(300);
        RuleFor(x => x.Dto.ImageUrl).MaximumLength(500);
    }
}

public class DeleteLostFoundCommandValidator : AbstractValidator<DeleteLostFoundCommand>
{
    public DeleteLostFoundCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}