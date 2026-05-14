using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Phantoms.Application.Products.Commands;
using Phantoms.Application.Products.DTOs;
using Phantoms.Application.Products.Queries;
using Phantoms.API.Authorization;
using Phantoms.Domain.Constants;

namespace Phantoms.API.Controllers;

[Authorize]
public class ProductsController(IMediator mediator) : BaseApiController
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? category = null)
    {
        var result = await mediator.Send(new GetProductsQuery(page, pageSize, category));
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await mediator.Send(new GetProductByIdQuery(id));
        return result.Succeeded ? Ok(result) : NotFound(result);
    }

    [HasPermission(Permissions.Products.Create)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
    {
        var result = await mediator.Send(new CreateProductCommand(dto));
        return result.Succeeded ? CreatedAtAction(nameof(GetById), new { id = result.Data }, result) : BadRequest(result);
    }

    [HasPermission(Permissions.Products.Edit)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductDto dto)
    {
        var result = await mediator.Send(new UpdateProductCommand(id, dto));
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HasPermission(Permissions.Products.Delete)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await mediator.Send(new DeleteProductCommand(id));
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}
