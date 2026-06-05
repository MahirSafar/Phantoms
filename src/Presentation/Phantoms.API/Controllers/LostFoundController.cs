using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Phantoms.API.Authorization;
using Phantoms.Application.LostFounds.Commands;
using Phantoms.Application.LostFounds.DTOs;
using Phantoms.Application.LostFounds.Queries;
using Phantoms.Domain.Constants;

namespace Phantoms.API.Controllers;

[Authorize]
public class LostFoundsController(IMediator mediator) : BaseApiController
{
    [HasPermission(Permissions.Students.View)]
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await mediator.Send(new GetLostFoundsQuery(page, pageSize));
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HasPermission(Permissions.Students.View)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await mediator.Send(new GetLostFoundByIdQuery(id));
        return result.Succeeded ? Ok(result) : NotFound(result);
    }

    [HasPermission(Permissions.Students.Create)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLostFoundDto dto)
    {
        var result = await mediator.Send(new CreateLostFoundCommand(dto));
        return result.Succeeded ? CreatedAtAction(nameof(GetById), new { id = result.Data }, result) : BadRequest(result);
    }

    [HasPermission(Permissions.Students.Edit)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateLostFoundDto dto)
    {
        var result = await mediator.Send(new UpdateLostFoundCommand(id, dto));
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HasPermission(Permissions.Students.Delete)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await mediator.Send(new DeleteLostFoundCommand(id));
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}