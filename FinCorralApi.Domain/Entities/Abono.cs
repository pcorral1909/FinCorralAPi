namespace FinCorralApi.Domain.Entities;

public class Abono
{
    public int Id { get; set; }
    public int PrestamoId { get; set; }
    public decimal Monto { get; set; }
    public DateTime Fecha { get; set; }
    public string Tipo { get; set; } = string.Empty;
    
    public Prestamo Prestamo { get; set; } = null!;
}