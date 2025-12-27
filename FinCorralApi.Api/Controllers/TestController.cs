using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace FinCorralApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
public class TestController : BaseController
{
    public TestController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet("public")]
    public IActionResult PublicEndpoint()
    {
        return Ok(new { message = "Este endpoint es público", timestamp = DateTime.UtcNow });
    }

    [HttpGet("protected")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public IActionResult ProtectedEndpoint()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        
        return Ok(new { 
            message = "Este endpoint está protegido", 
            userId = userId,
            email = email,
            timestamp = DateTime.UtcNow 
        });
    }
}