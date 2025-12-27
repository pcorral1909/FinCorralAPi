using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FinCorralApi.Application.Interfaces;
using FinCorralApi.Application.DTOs;
using FinCorralApi.Domain.Entities;
using MediatR;

namespace FinCorralApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[ApiExplorerSettings(IgnoreApi = true)]
public class ClientesController : BaseController
{
    private readonly IClienteRepository _clienteRepository;

    public ClientesController(IClienteRepository clienteRepository, IMediator mediator) : base(mediator)
    {
        _clienteRepository = clienteRepository;
    }

    [HttpPost]
    public async Task<IActionResult> CrearCliente([FromBody] CrearClienteDto dto)
    {
        var cliente = new Cliente
        {
            Nombre = dto.Nombre,
            Email = dto.Email,
            Telefono = dto.Telefono,
            FechaRegistro = DateTime.UtcNow
        };

        var clienteCreado = await _clienteRepository.CreateAsync(cliente);

        
        var response = new ClienteDto(
            clienteCreado.Id,
            clienteCreado.Nombre,
            clienteCreado.Email,
            clienteCreado.Telefono,
            clienteCreado.FechaRegistro
        );

        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerClientes()
    {
        var clientes = await _clienteRepository.GetAllAsync();
        var response = clientes.Select(c => new ClienteDto(
            c.Id,
            c.Nombre,
            c.Email,
            c.Telefono,
            c.FechaRegistro
        )).ToList();

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerCliente(int id)
    {
        var cliente = await _clienteRepository.GetByIdAsync(id);
        if (cliente == null)
            return NotFound();

        var response = new ClienteDto(
            cliente.Id,
            cliente.Nombre,
            cliente.Email,
            cliente.Telefono,
            cliente.FechaRegistro
        );

        return Ok(response);
    }
}