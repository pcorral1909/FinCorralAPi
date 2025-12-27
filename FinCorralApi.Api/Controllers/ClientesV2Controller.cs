using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FinCorralApi.Application.Commands;
using FinCorralApi.Application.DTOs;
using MediatR;

namespace FinCorralApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientesV2Controller : BaseController
{
    public ClientesV2Controller(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost]
    public async Task<IActionResult> CrearCliente([FromBody] CrearClienteDto dto)
    {
        var command = new CrearClienteCommand(dto.Nombre, dto.Email, dto.Telefono);
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerClientes()
    {
        var query = new GetAllClientesQuery();
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerCliente(int id)
    {
        var query = new GetClienteByIdQuery(id);
        var result = await Mediator.Send(query);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarCliente(int id)
    {
        var command = new EliminarClienteCommand(id);
        var result = await Mediator.Send(command);
        return Ok(result);
    }
}