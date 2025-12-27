namespace FinCorralApi.Domain.Entities;

public class Cliente
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; }
    
    public ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();
}