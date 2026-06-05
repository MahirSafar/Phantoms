using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Phantoms.Application.Common.Interfaces;
using Phantoms.Application.Common.Models;
using Phantoms.Application.Students.DTOs;

namespace Phantoms.Application.Students.Queries;

public record GetStudentsQuery(int Page = 1, int PageSize = 10)
    : IRequest<Result<PaginatedResult<StudentDto>>>;

public class GetStudentsQueryHandler(IAppDbContext context, IMapper mapper)
    : IRequestHandler<GetStudentsQuery, Result<PaginatedResult<StudentDto>>>
{
    public async Task<Result<PaginatedResult<StudentDto>>> Handle(GetStudentsQuery request, CancellationToken cancellationToken)
    {
        var query = context.Students
            .Where(s => !s.IsDeleted);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(s => s.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<StudentDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return Result<PaginatedResult<StudentDto>>.Success(new PaginatedResult<StudentDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        });
    }
}

public record GetStudentByIdQuery(Guid Id) : IRequest<Result<StudentDto>>;

public class GetStudentByIdQueryHandler(IAppDbContext context, IMapper mapper)
    : IRequestHandler<GetStudentByIdQuery, Result<StudentDto>>
{
    public async Task<Result<StudentDto>> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken)
    {
        var student = await context.Students
            .Where(s => s.Id == request.Id && !s.IsDeleted)
            .ProjectTo<StudentDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        if (student is null)
            return Result<StudentDto>.Failure("Student not found.");

        return Result<StudentDto>.Success(student);
    }
}