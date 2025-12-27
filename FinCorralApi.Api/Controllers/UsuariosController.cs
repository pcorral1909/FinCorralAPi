using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FinCorralApi.Application.Commands;
using FinCorralApi.Application.DTOs;
using MediatR;

namespace FinCorralApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsuariosController : BaseController
{
    public UsuariosController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost("step1")]
    public async Task<IActionResult> CrearUsuarioStep1([FromBody] CrearUsuarioStep1Dto dto)
    {
        var command = new CrearUsuarioStep1Command(dto.Telefono, dto.Email);
        var usuarioId = await Mediator.Send(command);
        return Ok(new { UsuarioId = usuarioId });
    }

    [HttpPost("step2")]
    public async Task<IActionResult> CrearUsuarioStep2([FromBody] CrearUsuarioStep2Dto dto)
    {
        var command = new CrearUsuarioStep2Command(dto.UsuarioId, dto.NombreCompleto, dto.FechaNacimiento, dto.Curp);
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("password")]
    public async Task<IActionResult> CrearPassword([FromBody] CrearPasswordDto dto)
    {
        var command = new CrearPasswordCommand(dto.UsuarioId, dto.Password);
        var result = await Mediator.Send(command);
        return Ok(result);
    }
}