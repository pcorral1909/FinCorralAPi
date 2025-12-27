namespace FinCorralApi.Domain.Entities;

public class Usuario
{
    public int Id { get; set; }
    public string Telefono { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? NombreCompleto { get; set; }
    public DateTime? FechaNacimiento { get; set; }
    public string? Curp { get; set; }
    public string? PasswordHash { get; set; }
    public bool Completado { get; set; }
    public bool TienePassword { get; set; }
    public DateTime FechaCreacion { get; set; }
}