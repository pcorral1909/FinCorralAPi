using MediatR;
using FinCorralApi.Application.DTOs;

namespace FinCorralApi.Application.Commands;

public record CrearClienteCommand(
    string Nombre,
    string Email,
    string Telefono
) : IRequest<ClienteDto>;

public record GetClienteByIdQuery(int Id) : IRequest<ClienteDto?>;
public record GetAllClientesQuery() : IRequest<List<ClienteDto>>;