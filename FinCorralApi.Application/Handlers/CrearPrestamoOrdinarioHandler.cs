using MediatR;
using FinCorralApi.Application.Commands;
using FinCorralApi.Application.DTOs;
using FinCorralApi.Application.Interfaces;
using FinCorralApi.Domain.Entities;
using FinCorralApi.Domain.Enums;

namespace FinCorralApi.Application.Handlers;

public class CrearPrestamoOrdinarioHandler : IRequestHandler<CrearPrestamoOrdinarioCommand, CrearPrestamoResponseDto>
{
    private readonly IPrestamoRepository _prestamoRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IAmortizacionService _amortizacionService;

    public CrearPrestamoOrdinarioHandler(
        IPrestamoRepository prestamoRepository,
        IClienteRepository clienteRepository,
        IAmortizacionService amortizacionService)
    {
        _prestamoRepository = prestamoRepository;
        _clienteRepository = clienteRepository;
        _amortizacionService = amortizacionService;
    }

    public async Task<CrearPrestamoResponseDto> Handle(CrearPrestamoOrdinarioCommand request, CancellationToken cancellationToken)
    {
        var cliente = await _clienteRepository.GetByIdAsync(request.ClienteId);
        if (cliente == null)
            throw new ArgumentException("Cliente no encontrado");

        var pagoQuincenal = (request.Monto * 175m) / 1000m;
        
        var prestamo = new Prestamo
        {
            ClienteId = request.ClienteId,
            Monto = request.Monto,
            PagoQuincenal = pagoQuincenal,
            FechaInicio = DateTime.UtcNow,
            FechaPrimerPago = request.FechaPrimerPago,
            FechaFin = request.FechaPrimerPago.AddMonths(4),
            TipoPrestamo = TipoPrestamo.Ordinario,
            InteresMensual = 0,
            Meses = 4
        };

        var prestamoCreado = await _prestamoRepository.CreateAsync(prestamo);
        var amortizaciones = _amortizacionService.CalcularAmortizacionOrdinaria(request.Monto, request.FechaPrimerPago);

        foreach (var amort in amortizaciones)
        {
            prestamoCreado.Amortizaciones.Add(new Amortizacion
            {
                PrestamoId = prestamoCreado.Id,
                NumeroPago = amort.NumeroPago,
                FechaPago = amort.FechaPago,
                MontoCapital = amort.MontoCapital,
                MontoInteres = amort.MontoInteres,
                MontoTotal = amort.MontoTotal,
                SaldoPendiente = amort.SaldoPendiente,
                Pagado = false
            });
        }

        await _prestamoRepository.UpdateAsync(prestamoCreado);

        return new CrearPrestamoResponseDto(
            new PrestamoResponseDto(
                prestamoCreado.Id,
                prestamoCreado.ClienteId,
                prestamoCreado.Monto,
                prestamoCreado.PagoQuincenal,
                prestamoCreado.FechaInicio,
                prestamoCreado.FechaPrimerPago,
                prestamoCreado.FechaFin,
                prestamoCreado.TipoPrestamo,
                prestamoCreado.InteresMensual,
                prestamoCreado.Meses
            ),
            amortizaciones
        );
    }
}