using FluentValidation;
using Phantoms.Application.TeamFinders.Commands;

namespace Phantoms.Application.TeamFinders.Validators;

public class CreateTeamFinderCommandValidator : AbstractValidator<CreateTeamFinderCommand>
{
    public CreateTeamFinderCommandValidator()
    {
        RuleFor(x => x.Dto.StudentId).NotEmpty();
        RuleFor(x => x.Dto.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Dto.Description).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Dto.ProjectName).MaximumLength(200);
        RuleFor(x => x.Dto.RequiredSkills).MaximumLength(500);
        RuleFor(x => x.Dto.NeededMembersCount).GreaterThan(0);
    }
}

public class UpdateTeamFinderCommandValidator : AbstractValidator<UpdateTeamFinderCommand>
{
    public UpdateTeamFinderCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Dto.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Dto.Description).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Dto.ProjectName).MaximumLength(200);
        RuleFor(x => x.Dto.RequiredSkills).MaximumLength(500);
        RuleFor(x => x.Dto.NeededMembersCount).GreaterThan(0);
    }
}

public class DeleteTeamFinderCommandValidator : AbstractValidator<DeleteTeamFinderCommand>
{
    public DeleteTeamFinderCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}