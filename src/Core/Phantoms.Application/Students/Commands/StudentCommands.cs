using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Phantoms.Application.Common.Interfaces;
using Phantoms.Application.Common.Models;
using Phantoms.Application.Students.DTOs;
using Phantoms.Domain.Entities;

namespace Phantoms.Application.Students.Commands;

public record CreateStudentCommand(CreateStudentDto Dto) : IRequest<Result<Guid>>;

public class CreateStudentCommandHandler(IAppDbContext context, IMapper mapper)
    : IRequestHandler<CreateStudentCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
    {
        var exists = await context.Students
            .AnyAsync(s => s.AppUserId == request.Dto.AppUserId, cancellationToken);

        if (exists)
            return Result<Guid>.Failure("Student profile already exists.");

        var student = mapper.Map<Student>(request.Dto);

        context.Students.Add(student);
        await context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(student.Id, "Student created successfully.");
    }
}

public record UpdateStudentCommand(Guid Id, UpdateStudentDto Dto) : IRequest<Result>;

public class UpdateStudentCommandHandler(IAppDbContext context, IMapper mapper)
    : IRequestHandler<UpdateStudentCommand, Result>
{
    public async Task<Result> Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
    {
        var student = await context.Students
            .FirstOrDefaultAsync(s => s.Id == request.Id && !s.IsDeleted, cancellationToken);

        if (student is null)
            return Result.Failure("Student not found.");

        mapper.Map(request.Dto, student);

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success("Student updated successfully.");
    }
}

public record DeleteStudentCommand(Guid Id) : IRequest<Result>;

public class DeleteStudentCommandHandler(IAppDbContext context)
    : IRequestHandler<DeleteStudentCommand, Result>
{
    public async Task<Result> Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
    {
        var student = await context.Students
            .FirstOrDefaultAsync(s => s.Id == request.Id && !s.IsDeleted, cancellationToken);

        if (student is null)
            return Result.Failure("Student not found.");

        student.IsDeleted = true;

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success("Student deleted successfully.");
    }
}