using Microsoft.AspNetCore.Mvc;

namespace Phantoms.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class BaseApiController : ControllerBase
{
}
