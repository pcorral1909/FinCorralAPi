using MediatR;
using FinCorralApi.Application.DTOs;

namespace FinCorralApi.Application.Commands;

public record CrearPrestamoConTasaCommand(
    int ClienteId,
    decimal Monto,
    decimal TasaInteres,
    DateTime FechaPrimerPago
) : IRequest<CrearPrestamoResponseDto>;

public record CrearPrestamoMSICommand(
    int ClienteId,
    decimal Monto,
    int Meses,
    DateTime FechaPrimerPago
) : IRequest<CrearPrestamoResponseDto>;

public record CrearPrestamoLibreCommand(
    int ClienteId,
    decimal Monto
) : IRequest<PrestamoResponseDto>;

public record RegistrarAbonoCommand(
    int PrestamoId,
    decimal Monto
) : IRequest<string>;

public record PagarAmortizacionCommand(
    int PrestamoId,
    int NumeroPago,
    decimal MontoPagado
) : IRequest<string>;