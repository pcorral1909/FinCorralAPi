using FinCorralApi.Domain.Enums;

namespace FinCorralApi.Application.DTOs;

public record PrestamoResponseDto(
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

public record CrearPrestamoResponseDto(
    PrestamoResponseDto Prestamo,
    List<AmortizacionDto> Amortizaciones
);