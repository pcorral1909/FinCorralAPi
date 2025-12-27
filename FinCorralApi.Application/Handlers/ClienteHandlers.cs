using MediatR;
using FinCorralApi.Application.Commands;
using FinCorralApi.Application.DTOs;
using FinCorralApi.Application.Interfaces;
using FinCorralApi.Domain.Entities;

namespace FinCorralApi.Application.Handlers;

public class CrearClienteHandler : IRequestHandler<CrearClienteCommand, ClienteDto>
{
    private readonly IClienteRepository _clienteRepository;

    public CrearClienteHandler(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<ClienteDto> Handle(CrearClienteCommand request, CancellationToken cancellationToken)
    {
        var cliente = new Cliente
        {
            Nombre = request.Nombre,
            Email = request.Email,
            Telefono = request.Telefono,
            FechaRegistro = DateTime.UtcNow
        };

        var clienteCreado = await _clienteRepository.CreateAsync(cliente);

        return new ClienteDto(
            clienteCreado.Id,
            clienteCreado.Nombre,
            clienteCreado.Email,
            clienteCreado.Telefono,
            clienteCreado.FechaRegistro
        );
    }
}

public class GetClienteByIdHandler : IRequestHandler<GetClienteByIdQuery, ClienteDto?>
{
    private readonly IClienteRepository _clienteRepository;

    public GetClienteByIdHandler(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<ClienteDto?> Handle(GetClienteByIdQuery request, CancellationToken cancellationToken)
    {
        var cliente = await _clienteRepository.GetByIdAsync(request.Id);
        if (cliente == null) return null;

        return new ClienteDto(
            cliente.Id,
            cliente.Nombre,
            cliente.Email,
            cliente.Telefono,
            cliente.FechaRegistro
        );
    }
}

public class GetAllClientesHandler : IRequestHandler<GetAllClientesQuery, List<ClienteDto>>
{
    private readonly IClienteRepository _clienteRepository;

    public GetAllClientesHandler(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<List<ClienteDto>> Handle(GetAllClientesQuery request, CancellationToken cancellationToken)
    {
        var clientes = await _clienteRepository.GetAllAsync();
        
        return clientes.Select(c => new ClienteDto(
            c.Id,
            c.Nombre,
            c.Email,
            c.Telefono,
            c.FechaRegistro
        )).ToList();
    }
}