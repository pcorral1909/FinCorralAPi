using FinCorralApi.Domain.Entities;
using FinCorralApi.Application.DTOs;

namespace FinCorralApi.Application.Interfaces;

public interface IAmortizacionService
{
    List<AmortizacionDto> CalcularAmortizacionOrdinaria(decimal monto, DateTime fechaPrimerPago);
    List<AmortizacionDto> CalcularAmortizacionConTasa(decimal monto, decimal tasaInteres, DateTime fechaPrimerPago);
    Task RecalcularAmortizacionConTasa(int prestamoId, decimal nuevoCapital, DateTime fechaAbono);
}