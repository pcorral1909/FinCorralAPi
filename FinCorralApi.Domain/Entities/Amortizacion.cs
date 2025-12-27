namespace FinCorralApi.Domain.Entities;

public class Amortizacion
{
    public int Id { get; set; }
    public int PrestamoId { get; set; }
    public int NumeroPago { get; set; }
    public DateTime FechaPago { get; set; }
    public decimal MontoCapital { get; set; }
    public decimal MontoInteres { get; set; }
    public decimal MontoTotal { get; set; }
    public decimal SaldoPendiente { get; set; }
    public bool Pagado { get; set; }
    
    public Prestamo Prestamo { get; set; } = null!;
}