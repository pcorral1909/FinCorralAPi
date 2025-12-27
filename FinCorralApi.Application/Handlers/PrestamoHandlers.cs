using MediatR;
using FinCorralApi.Application.Commands;
using FinCorralApi.Application.DTOs;
using FinCorralApi.Application.Interfaces;
using FinCorralApi.Domain.Entities;
using FinCorralApi.Domain.Enums;

namespace FinCorralApi.Application.Handlers;

public class CrearPrestamoConTasaHandler : IRequestHandler<CrearPrestamoConTasaCommand, CrearPrestamoResponseDto>
{
    private readonly IPrestamoRepository _prestamoRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IAmortizacionService _amortizacionService;

    public CrearPrestamoConTasaHandler(
        IPrestamoRepository prestamoRepository,
        IClienteRepository clienteRepository,
        IAmortizacionService amortizacionService)
    {
        _prestamoRepository = prestamoRepository;
        _clienteRepository = clienteRepository;
        _amortizacionService = amortizacionService;
    }

    public async Task<CrearPrestamoResponseDto> Handle(CrearPrestamoConTasaCommand request, CancellationToken cancellationToken)
    {
        var cliente = await _clienteRepository.GetByIdAsync(request.ClienteId);
        if (cliente == null)
            throw new ArgumentException("Cliente no encontrado");

        var prestamo = new Prestamo
        {
            ClienteId = request.ClienteId,
            Monto = request.Monto,
            PagoQuincenal = 0,
            FechaInicio = DateTime.UtcNow,
            FechaPrimerPago = request.FechaPrimerPago,
            FechaFin = request.FechaPrimerPago.AddMonths(12),
            TipoPrestamo = TipoPrestamo.ConTasa,
            InteresMensual = request.TasaInteres,
            Meses = 12
        };

        var prestamoCreado = await _prestamoRepository.CreateAsync(prestamo);
        var amortizaciones = _amortizacionService.CalcularAmortizacionConTasa(request.Monto, request.TasaInteres, request.FechaPrimerPago);

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

public class RegistrarAbonoHandler : IRequestHandler<RegistrarAbonoCommand, string>
{
    private readonly IPrestamoRepository _prestamoRepository;
    private readonly IAmortizacionService _amortizacionService;

    public RegistrarAbonoHandler(IPrestamoRepository prestamoRepository, IAmortizacionService amortizacionService)
    {
        _prestamoRepository = prestamoRepository;
        _amortizacionService = amortizacionService;
    }

    public async Task<string> Handle(RegistrarAbonoCommand request, CancellationToken cancellationToken)
    {
        var prestamo = await _prestamoRepository.GetByIdAsync(request.PrestamoId);
        if (prestamo == null)
            throw new ArgumentException("Préstamo no encontrado");

        var abono = new Abono
        {
            PrestamoId = request.PrestamoId,
            Monto = request.Monto,
            Fecha = DateTime.UtcNow,
            Tipo = "Capital"
        };

        prestamo.Abonos.Add(abono);

        if (prestamo.TipoPrestamo == TipoPrestamo.ConTasa)
        {
            var totalAbonosCapital = prestamo.Abonos
                .Where(a => a.Tipo == "Capital")
                .Sum(a => a.Monto);
            var nuevoCapital = prestamo.Monto - totalAbonosCapital;
            await _amortizacionService.RecalcularAmortizacionConTasa(request.PrestamoId, nuevoCapital, DateTime.UtcNow);
        }

        await _prestamoRepository.UpdateAsync(prestamo);
        return "Abono registrado exitosamente";
    }
}

public class PagarAmortizacionHandler : IRequestHandler<PagarAmortizacionCommand, string>
{
    private readonly IPrestamoRepository _prestamoRepository;

    public PagarAmortizacionHandler(IPrestamoRepository prestamoRepository)
    {
        _prestamoRepository = prestamoRepository;
    }

    public async Task<string> Handle(PagarAmortizacionCommand request, CancellationToken cancellationToken)
    {
        var prestamo = await _prestamoRepository.GetByIdAsync(request.PrestamoId);
        if (prestamo == null)
            throw new ArgumentException("Préstamo no encontrado");

        var amortizacion = prestamo.Amortizaciones
            .FirstOrDefault(a => a.NumeroPago == request.NumeroPago && !a.Pagado);
        
        if (amortizacion == null)
            throw new ArgumentException("Amortización no encontrada o ya pagada");

        amortizacion.Pagado = true;

        var abono = new Abono
        {
            PrestamoId = request.PrestamoId,
            Monto = request.MontoPagado,
            Fecha = DateTime.UtcNow,
            Tipo = "Pago Amortización"
        };

        prestamo.Abonos.Add(abono);
        await _prestamoRepository.UpdateAsync(prestamo);

        return "Amortización pagada exitosamente";
    }
}