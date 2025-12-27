using MediatR;
using FinCorralApi.Application.Queries;
using FinCorralApi.Application.DTOs;
using FinCorralApi.Application.Interfaces;

namespace FinCorralApi.Application.Handlers;

public class GetPrestamosByClienteHandler : IRequestHandler<GetPrestamosByClienteQuery, List<PrestamoConAmortizacionDto>>
{
    private readonly IPrestamoRepository _prestamoRepository;

    public GetPrestamosByClienteHandler(IPrestamoRepository prestamoRepository)
    {
        _prestamoRepository = prestamoRepository;
    }

    public async Task<List<PrestamoConAmortizacionDto>> Handle(GetPrestamosByClienteQuery request, CancellationToken cancellationToken)
    {
        var prestamos = await _prestamoRepository.GetByClienteIdAsync(request.ClienteId);
        
        return prestamos.Select(p => new PrestamoConAmortizacionDto(
            p.Id,
            p.ClienteId,
            p.Monto,
            p.PagoQuincenal,
            p.FechaInicio,
            p.FechaPrimerPago,
            p.FechaFin,
            p.TipoPrestamo,
            p.InteresMensual,
            p.Meses,
            p.Amortizaciones.Select(a => new AmortizacionDto(
                a.NumeroPago,
                a.FechaPago,
                a.MontoCapital,
                a.MontoInteres,
                a.MontoTotal,
                a.SaldoPendiente,
                a.Pagado
            )).OrderBy(a => a.NumeroPago).ToList()
        )).ToList();
    }
}