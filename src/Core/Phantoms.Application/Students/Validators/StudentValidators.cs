using FluentValidation;
using Phantoms.Application.Students.Commands;

namespace Phantoms.Application.Students.Validators;

public class CreateStudentCommandValidator : AbstractValidator<CreateStudentCommand>
{
    public CreateStudentCommandValidator()
    {
        RuleFor(x => x.Dto.AppUserId).NotEmpty();
        RuleFor(x => x.Dto.University).MaximumLength(200);
        RuleFor(x => x.Dto.Faculty).MaximumLength(200);
        RuleFor(x => x.Dto.Specialty).MaximumLength(200);
        RuleFor(x => x.Dto.Bio).MaximumLength(1000);
    }
}

public class UpdateStudentCommandValidator : AbstractValidator<UpdateStudentCommand>
{
    public UpdateStudentCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Dto.University).MaximumLength(200);
        RuleFor(x => x.Dto.Faculty).MaximumLength(200);
        RuleFor(x => x.Dto.Specialty).MaximumLength(200);
        RuleFor(x => x.Dto.Bio).MaximumLength(1000);
    }
}

public class DeleteStudentCommandValidator : AbstractValidator<DeleteStudentCommand>
{
    public DeleteStudentCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}