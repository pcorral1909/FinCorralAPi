using MediatR;
using FinCorralApi.Application.DTOs;

namespace FinCorralApi.Application.Commands;

public record CrearPrestamoOrdinarioCommand(
    int ClienteId,
    decimal Monto,
    DateTime FechaPrimerPago
) : IRequest<CrearPrestamoResponseDto>;