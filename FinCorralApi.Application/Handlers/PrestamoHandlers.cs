using MediatR;
using FinCorralApi.Application.Commands;
using FinCorralApi.Application.Queries;
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

public class CrearPrestamoMSIHandler : IRequestHandler<CrearPrestamoMSICommand, CrearPrestamoResponseDto>
{
    private readonly IPrestamoRepository _prestamoRepository;
    private readonly IClienteRepository _clienteRepository;

    public CrearPrestamoMSIHandler(IPrestamoRepository prestamoRepository, IClienteRepository clienteRepository)
    {
        _prestamoRepository = prestamoRepository;
        _clienteRepository = clienteRepository;
    }

    public async Task<CrearPrestamoResponseDto> Handle(CrearPrestamoMSICommand request, CancellationToken cancellationToken)
    {
        var cliente = await _clienteRepository.GetByIdAsync(request.ClienteId);
        if (cliente == null) throw new ArgumentException("Cliente no encontrado");

        var pagoMensual = request.Monto / request.Meses;
        
        var prestamo = new Prestamo
        {
            ClienteId = request.ClienteId,
            Monto = request.Monto,
            PagoQuincenal = pagoMensual,
            FechaInicio = DateTime.UtcNow,
            FechaPrimerPago = request.FechaPrimerPago,
            FechaFin = request.FechaPrimerPago.AddMonths(request.Meses),
            TipoPrestamo = TipoPrestamo.MSI,
            InteresMensual = 0,
            Meses = request.Meses
        };

        var prestamoCreado = await _prestamoRepository.CreateAsync(prestamo);
        var amortizaciones = new List<AmortizacionDto>();
        var fechaPago = request.FechaPrimerPago;

        for (int i = 1; i <= request.Meses; i++)
        {
            var amortizacion = new Amortizacion
            {
                PrestamoId = prestamoCreado.Id,
                NumeroPago = i,
                FechaPago = fechaPago,
                MontoCapital = pagoMensual,
                MontoInteres = 0,
                MontoTotal = pagoMensual,
                SaldoPendiente = request.Monto - (pagoMensual * i),
                Pagado = false
            };
            
            prestamoCreado.Amortizaciones.Add(amortizacion);
            amortizaciones.Add(new AmortizacionDto(i, fechaPago, pagoMensual, 0, pagoMensual, request.Monto - (pagoMensual * i), false));
            fechaPago = fechaPago.AddMonths(1);
        }

        await _prestamoRepository.UpdateAsync(prestamoCreado);

        return new CrearPrestamoResponseDto(
            new PrestamoResponseDto(
                prestamoCreado.Id, prestamoCreado.ClienteId, prestamoCreado.Monto,
                prestamoCreado.PagoQuincenal, prestamoCreado.FechaInicio, prestamoCreado.FechaPrimerPago,
                prestamoCreado.FechaFin, prestamoCreado.TipoPrestamo, prestamoCreado.InteresMensual, prestamoCreado.Meses
            ),
            amortizaciones
        );
    }
}

public class CrearPrestamoLibreHandler : IRequestHandler<CrearPrestamoLibreCommand, PrestamoResponseDto>
{
    private readonly IPrestamoRepository _prestamoRepository;
    private readonly IClienteRepository _clienteRepository;

    public CrearPrestamoLibreHandler(IPrestamoRepository prestamoRepository, IClienteRepository clienteRepository)
    {
        _prestamoRepository = prestamoRepository;
        _clienteRepository = clienteRepository;
    }

    public async Task<PrestamoResponseDto> Handle(CrearPrestamoLibreCommand request, CancellationToken cancellationToken)
    {
        var cliente = await _clienteRepository.GetByIdAsync(request.ClienteId);
        if (cliente == null) throw new ArgumentException("Cliente no encontrado");

        var prestamo = new Prestamo
        {
            ClienteId = request.ClienteId,
            Monto = request.Monto,
            PagoQuincenal = 0,
            FechaInicio = DateTime.UtcNow,
            FechaPrimerPago = DateTime.UtcNow,
            FechaFin = DateTime.UtcNow.AddYears(1),
            TipoPrestamo = TipoPrestamo.Libre,
            InteresMensual = 0,
            Meses = 0
        };

        var prestamoCreado = await _prestamoRepository.CreateAsync(prestamo);

        return new PrestamoResponseDto(
            prestamoCreado.Id, prestamoCreado.ClienteId, prestamoCreado.Monto,
            prestamoCreado.PagoQuincenal, prestamoCreado.FechaInicio, prestamoCreado.FechaPrimerPago,
            prestamoCreado.FechaFin, prestamoCreado.TipoPrestamo, prestamoCreado.InteresMensual, prestamoCreado.Meses
        );
    }
}

public class GetTiposPrestamoHandler : IRequestHandler<GetTiposPrestamoQuery, List<TipoPrestamoDto>>
{
    public Task<List<TipoPrestamoDto>> Handle(GetTiposPrestamoQuery request, CancellationToken cancellationToken)
    {
        var tipos = new List<TipoPrestamoDto>
        {
            new(1, "Ordinario", "175 por cada 1000 pesos a 8 quincenas"),
            new(2, "Con Tasa", "Préstamo con tasa de interés mensual variable"),
            new(3, "MSI", "Meses sin intereses - pagos fijos mensuales"),
            new(4, "Libre", "Préstamo libre sin tabla de amortización")
        };
        
        return Task.FromResult(tipos);
    }
}