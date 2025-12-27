using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FinCorralApi.Application.Interfaces;
using FinCorralApi.Application.DTOs;
using FinCorralApi.Domain.Entities;
using FinCorralApi.Domain.Enums;
using MediatR;

namespace FinCorralApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[ApiExplorerSettings(IgnoreApi = true)]
public class PrestamosController : BaseController
{
    private readonly IPrestamoRepository _prestamoRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IAmortizacionService _amortizacionService;

    public PrestamosController(
        IPrestamoRepository prestamoRepository,
        IClienteRepository clienteRepository,
        IAmortizacionService amortizacionService,
        IMediator mediator) : base(mediator)
    {
        _prestamoRepository = prestamoRepository;
        _clienteRepository = clienteRepository;
        _amortizacionService = amortizacionService;
    }

    [HttpPost("ordinario")]
    public async Task<IActionResult> CrearPrestamoOrdinario([FromBody] CrearPrestamoOrdinarioDto dto)
    {
        var cliente = await _clienteRepository.GetByIdAsync(dto.ClienteId);
        if (cliente == null)
            return BadRequest("Cliente no encontrado");

        var pagoQuincenal = (dto.Monto * 175m) / 1000m;
        
        var prestamo = new Prestamo
        {
            ClienteId = dto.ClienteId,
            Monto = dto.Monto,
            PagoQuincenal = pagoQuincenal,
            FechaInicio = DateTime.UtcNow,
            FechaPrimerPago = dto.FechaPrimerPago,
            FechaFin = dto.FechaPrimerPago.AddMonths(4), // 8 quincenas = 4 meses
            TipoPrestamo = TipoPrestamo.Ordinario,
            InteresMensual = 0,
            Meses = 4
        };

        var prestamoCreado = await _prestamoRepository.CreateAsync(prestamo);

        // Generar tabla de amortización
        var amortizaciones = _amortizacionService.CalcularAmortizacionOrdinaria(dto.Monto, dto.FechaPrimerPago);

        // Guardar amortizaciones en BD
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

        var response = new CrearPrestamoResponseDto(
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

        return Ok(response);
    }

    [HttpPost("con-tasa")]
    public async Task<IActionResult> CrearPrestamoConTasa([FromBody] CrearPrestamoConTasaDto dto)
    {
        var cliente = await _clienteRepository.GetByIdAsync(dto.ClienteId);
        if (cliente == null)
            return BadRequest("Cliente no encontrado");

        var prestamo = new Prestamo
        {
            ClienteId = dto.ClienteId,
            Monto = dto.Monto,
            PagoQuincenal = 0,
            FechaInicio = DateTime.UtcNow,
            FechaPrimerPago = dto.FechaPrimerPago,
            FechaFin = dto.FechaPrimerPago.AddMonths(12),
            TipoPrestamo = TipoPrestamo.ConTasa,
            InteresMensual = dto.TasaInteres,
            Meses = 12
        };

        var prestamoCreado = await _prestamoRepository.CreateAsync(prestamo);

        // Generar tabla de amortización
        var amortizaciones = _amortizacionService.CalcularAmortizacionConTasa(dto.Monto, dto.TasaInteres, dto.FechaPrimerPago);

        // Guardar amortizaciones en BD
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

        var responseConTasa = new CrearPrestamoResponseDto(
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

        return Ok(responseConTasa);
    }

    [HttpGet("cliente/{clienteId}")]
    public async Task<IActionResult> ObtenerPrestamosPorCliente(int clienteId)
    {
        var prestamos = await _prestamoRepository.GetByClienteIdAsync(clienteId);
        return Ok(prestamos);
    }

    [HttpPost("{prestamoId}/abono")]
    public async Task<IActionResult> RegistrarAbono(int prestamoId, [FromBody] decimal monto)
    {
        var prestamo = await _prestamoRepository.GetByIdAsync(prestamoId);
        if (prestamo == null)
            return NotFound();

        var abono = new Abono
        {
            PrestamoId = prestamoId,
            Monto = monto,
            Fecha = DateTime.UtcNow,
            Tipo = "Capital"
        };

        prestamo.Abonos.Add(abono);

        // Si es préstamo con tasa, recalcular amortización
        if (prestamo.TipoPrestamo == TipoPrestamo.ConTasa)
        {
            var totalAbonosCapital = prestamo.Abonos
                .Where(a => a.Tipo == "Capital")
                .Sum(a => a.Monto);
            var nuevoCapital = prestamo.Monto - totalAbonosCapital;
            await _amortizacionService.RecalcularAmortizacionConTasa(prestamoId, nuevoCapital, DateTime.UtcNow);
        }

        await _prestamoRepository.UpdateAsync(prestamo);

        return Ok("Abono registrado exitosamente");
    }

    [HttpPost("{prestamoId}/pagar-amortizacion")]
    public async Task<IActionResult> PagarAmortizacion([FromBody] PagarAmortizacionDto dto)
    {
        var prestamo = await _prestamoRepository.GetByIdAsync(dto.PrestamoId);
        if (prestamo == null)
            return NotFound();

        var amortizacion = prestamo.Amortizaciones
            .FirstOrDefault(a => a.NumeroPago == dto.NumeroPago && !a.Pagado);
        
        if (amortizacion == null)
            return BadRequest("Amortización no encontrada o ya pagada");

        // Marcar como pagado
        amortizacion.Pagado = true;

        // Registrar el pago como abono
        var abono = new Abono
        {
            PrestamoId = dto.PrestamoId,
            Monto = dto.MontoPagado,
            Fecha = DateTime.UtcNow,
            Tipo = "Pago Amortización"
        };

        prestamo.Abonos.Add(abono);
        await _prestamoRepository.UpdateAsync(prestamo);

        return Ok("Amortización pagada exitosamente");
    }
}