namespace FinCorralApi.Application.DTOs;

public record CrearClienteDto(
    string Nombre,
    string Email,
    string Telefono
);

public record ClienteDto(
    int Id,
    string Nombre,
    string Email,
    string Telefono,
    DateTime FechaRegistro
);