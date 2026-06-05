using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Phantoms.API.Authorization;
using Phantoms.Application.Announcements.Commands;
using Phantoms.Application.Announcements.DTOs;
using Phantoms.Application.Announcements.Queries;
using Phantoms.Domain.Constants;
using Phantoms.Domain.Enums;

namespace Phantoms.API.Controllers;

[Authorize]
public class AnnouncementsController(IMediator mediator) : BaseApiController
{
    [HasPermission(Permissions.Announcements.View)]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] AnnouncementCategory? category = null)
    {
        var result = await mediator.Send(new GetAnnouncementsQuery(page, pageSize, Category: category));
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HasPermission(Permissions.Announcements.Publish)]
    [HttpGet("review")]
    public async Task<IActionResult> GetForReview(
        [FromQuery] PublishStatus status = PublishStatus.Pending,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] AnnouncementCategory? category = null)
    {
        var result = await mediator.Send(new GetAnnouncementsQuery(page, pageSize, status, category, ApplyCurrentTeacherScope: false));
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HasPermission(Permissions.Announcements.View)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await mediator.Send(new GetAnnouncementByIdQuery(id));
        return result.Succeeded ? Ok(result) : NotFound(result);
    }

    [HasPermission(Permissions.Announcements.Create)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAnnouncementDto dto)
    {
        var result = await mediator.Send(new CreateAnnouncementCommand(dto));
        return result.Succeeded ? CreatedAtAction(nameof(GetById), new { id = result.Data }, result) : BadRequest(result);
    }

    [HasPermission(Permissions.Announcements.Edit)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAnnouncementDto dto)
    {
        var result = await mediator.Send(new UpdateAnnouncementCommand(id, dto));
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HasPermission(Permissions.Announcements.Delete)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await mediator.Send(new DeleteAnnouncementCommand(id));
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HasPermission(Permissions.Announcements.Publish)]
    [HttpPost("{id:guid}/publish")]
    public async Task<IActionResult> Publish(Guid id)
    {
        var result = await mediator.Send(new PublishAnnouncementCommand(id));
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HasPermission(Permissions.Announcements.Publish)]
    [HttpPost("{id:guid}/reject")]
    public async Task<IActionResult> Reject(Guid id)
    {
        var result = await mediator.Send(new RejectAnnouncementCommand(id));
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}
