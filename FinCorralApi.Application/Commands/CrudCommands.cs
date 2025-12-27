using MediatR;
using FinCorralApi.Application.DTOs;

namespace FinCorralApi.Application.Commands;

// Cliente Commands
public record EliminarClienteCommand(int Id) : IRequest<string>;

// Prestamo Commands  
public record EliminarPrestamoCommand(int Id) : IRequest<string>;
public record EditarPrestamoCommand(int Id, decimal Monto, DateTime FechaPrimerPago) : IRequest<PrestamoResponseDto>;
public record LiquidarPrestamoCommand(int Id) : IRequest<string>;

// Usuario Commands
public record CrearUsuarioStep1Command(string Telefono, string Email) : IRequest<int>;
public record CrearUsuarioStep2Command(int UsuarioId, string NombreCompleto, DateTime FechaNacimiento, string Curp) : IRequest<UsuarioDto>;
public record CrearPasswordCommand(int UsuarioId, string Password) : IRequest<string>;