using FinCorralApi.Domain.Enums;

namespace FinCorralApi.Domain.Entities;

public class Prestamo
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public decimal Monto { get; set; }
    public decimal PagoQuincenal { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaPrimerPago { get; set; }
    public DateTime FechaFin { get; set; }
    public TipoPrestamo TipoPrestamo { get; set; }
    public decimal InteresMensual { get; set; }
    public int Meses { get; set; }
    
    public Cliente Cliente { get; set; } = null!;
    public ICollection<Abono> Abonos { get; set; } = new List<Abono>();
    public ICollection<Amortizacion> Amortizaciones { get; set; } = new List<Amortizacion>();
}