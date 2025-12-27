using FinCorralApi.Domain.Enums;

namespace FinCorralApi.Application.DTOs;

public record CrearPrestamoOrdinarioDto(
    int ClienteId,
    decimal Monto,
    DateTime FechaPrimerPago
);

public record CrearPrestamoConTasaDto(
    int ClienteId,
    decimal Monto,
    decimal TasaInteres,
    DateTime FechaPrimerPago
);

public record PrestamoDto(
    int Id,
    int ClienteId,
    decimal Monto,
    decimal PagoQuincenal,
    DateTime FechaInicio,
    DateTime FechaPrimerPago,
    DateTime FechaFin,
    TipoPrestamo TipoPrestamo,
    decimal InteresMensual,
    int Meses
);

public record AmortizacionDto(
    int NumeroPago,
    DateTime FechaPago,
    decimal MontoCapital,
    decimal MontoInteres,
    decimal MontoTotal,
    decimal SaldoPendiente,
    bool Pagado
);