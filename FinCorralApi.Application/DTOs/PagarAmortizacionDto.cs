namespace FinCorralApi.Application.DTOs;

public record PagarAmortizacionDto(
    int PrestamoId,
    int NumeroPago,
    decimal MontoPagado
);