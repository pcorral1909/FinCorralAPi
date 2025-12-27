namespace FinCorralApi.Application.DTOs;

public record CrearUsuarioStep1Dto(string Telefono, string Email);
public record CrearUsuarioStep2Dto(int UsuarioId, string NombreCompleto, DateTime FechaNacimiento, string Curp);
public record CrearPasswordDto(int UsuarioId, string Password);
public record UsuarioDto(int Id, string Telefono, string Email, string? NombreCompleto, DateTime? FechaNacimiento, string? Curp, bool Completado, bool TienePassword);