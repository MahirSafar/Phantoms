using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Phantoms.API.Authorization;
using Phantoms.Application.Events.Commands;
using Phantoms.Application.Events.DTOs;
using Phantoms.Application.Events.Queries;
using Phantoms.Domain.Constants;
using Phantoms.Domain.Enums;

namespace Phantoms.API.Controllers;

[Authorize]
public class EventsController(IMediator mediator) : BaseApiController
{
    [HasPermission(Permissions.Events.View)]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await mediator.Send(new GetEventsQuery(page, pageSize));
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HasPermission(Permissions.Events.Publish)]
    [HttpGet("review")]
    public async Task<IActionResult> GetForReview(
        [FromQuery] PublishStatus status = PublishStatus.Pending,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await mediator.Send(new GetEventsQuery(page, pageSize, status, ApplyCurrentTeacherScope: false));
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HasPermission(Permissions.Events.View)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await mediator.Send(new GetEventByIdQuery(id));
        return result.Succeeded ? Ok(result) : NotFound(result);
    }

    [HasPermission(Permissions.Events.Create)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEventDto dto)
    {
        var result = await mediator.Send(new CreateEventCommand(dto));
        return result.Succeeded ? CreatedAtAction(nameof(GetById), new { id = result.Data }, result) : BadRequest(result);
    }

    [HasPermission(Permissions.Events.Edit)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEventDto dto)
    {
        var result = await mediator.Send(new UpdateEventCommand(id, dto));
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HasPermission(Permissions.Events.Delete)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await mediator.Send(new DeleteEventCommand(id));
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HasPermission(Permissions.Events.Publish)]
    [HttpPost("{id:guid}/publish")]
    public async Task<IActionResult> Publish(Guid id)
    {
        var result = await mediator.Send(new PublishEventCommand(id));
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HasPermission(Permissions.Events.Publish)]
    [HttpPost("{id:guid}/reject")]
    public async Task<IActionResult> Reject(Guid id)
    {
        var result = await mediator.Send(new RejectEventCommand(id));
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}
