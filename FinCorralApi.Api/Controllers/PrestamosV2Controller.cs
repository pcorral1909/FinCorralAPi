using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FinCorralApi.Application.Commands;
using FinCorralApi.Application.Queries;
using FinCorralApi.Application.DTOs;
using MediatR;

namespace FinCorralApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PrestamosV2Controller : BaseController
{
    public PrestamosV2Controller(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost("ordinario")]
    public async Task<IActionResult> CrearPrestamoOrdinario([FromBody] CrearPrestamoOrdinarioDto dto)
    {
        var command = new CrearPrestamoOrdinarioCommand(dto.ClienteId, dto.Monto, dto.FechaPrimerPago);
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("con-tasa")]
    public async Task<IActionResult> CrearPrestamoConTasa([FromBody] CrearPrestamoConTasaDto dto)
    {
        var command = new CrearPrestamoConTasaCommand(dto.ClienteId, dto.Monto, dto.TasaInteres, dto.FechaPrimerPago);
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("cliente/{clienteId}")]
    public async Task<IActionResult> ObtenerPrestamosPorCliente(int clienteId)
    {
        var query = new GetPrestamosByClienteQuery(clienteId);
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("{prestamoId}/abono")]
    public async Task<IActionResult> RegistrarAbono(int prestamoId, [FromBody] decimal monto)
    {
        var command = new RegistrarAbonoCommand(prestamoId, monto);
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("{prestamoId}/pagar-amortizacion")]
    public async Task<IActionResult> PagarAmortizacion(int prestamoId, [FromBody] PagarAmortizacionDto dto)
    {
        var command = new PagarAmortizacionCommand(prestamoId, dto.NumeroPago, dto.MontoPagado);
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarPrestamo(int id)
    {
        var command = new EliminarPrestamoCommand(id);
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("{id}/liquidar")]
    public async Task<IActionResult> LiquidarPrestamo(int id)
    {
        var command = new LiquidarPrestamoCommand(id);
        var result = await Mediator.Send(command);
        return Ok(result);
    }
}