using FinCorralApi.Application.Interfaces;
using FinCorralApi.Application.DTOs;

namespace FinCorralApi.Application.Services;

public class AmortizacionService : IAmortizacionService
{
    private readonly IPrestamoRepository _prestamoRepository;

    public AmortizacionService(IPrestamoRepository prestamoRepository)
    {
        _prestamoRepository = prestamoRepository;
    }

    public List<AmortizacionDto> CalcularAmortizacionOrdinaria(decimal monto, DateTime fechaPrimerPago)
    {
        var amortizaciones = new List<AmortizacionDto>();
        var pagoQuincenal = (monto * 175m) / 1000m; // 175 por cada 1000
        var fechaPago = fechaPrimerPago;

        for (int i = 1; i <= 8; i++)
        {
            amortizaciones.Add(new AmortizacionDto(
                NumeroPago: i,
                FechaPago: fechaPago,
                MontoCapital: monto / 8m, // Capital dividido en 8 pagos
                MontoInteres: pagoQuincenal - (monto / 8m),
                MontoTotal: pagoQuincenal,
                SaldoPendiente: (pagoQuincenal * (8-i)),
                Pagado: false
            ));

            // Siguiente pago quincenal (1 o 15 del mes)
            if (fechaPago.Day == 1)
            {
                fechaPago = new DateTime(fechaPago.Year, fechaPago.Month, 15);
            }
            else
            {
                fechaPago = fechaPago.AddMonths(1);
                fechaPago = new DateTime(fechaPago.Year, fechaPago.Month, 1);
            }
        }

        return amortizaciones;
    }

    public List<AmortizacionDto> CalcularAmortizacionConTasa(decimal monto, decimal tasaInteres, DateTime fechaPrimerPago)
    {
        var amortizaciones = new List<AmortizacionDto>();
        var fechaPago = fechaPrimerPago;

        for (int i = 1; i <= 12; i++)
        {
            var interesMensual = monto * (tasaInteres / 100m);
            
            amortizaciones.Add(new AmortizacionDto(
                NumeroPago: i,
                FechaPago: fechaPago,
                MontoCapital: 0m, // Solo intereses, capital no se reduce automáticamente
                MontoInteres: interesMensual,
                MontoTotal: interesMensual,
                SaldoPendiente: monto,
                Pagado: false
            ));

            fechaPago = fechaPago.AddMonths(1);
        }

        return amortizaciones;
    }

    public async Task RecalcularAmortizacionConTasa(int prestamoId, decimal nuevoCapital, DateTime fechaAbono)
    {
        var prestamo = await _prestamoRepository.GetByIdAsync(prestamoId);
        if (prestamo == null) return;

        // Actualizar amortizaciones futuras con el nuevo capital
        var amortizacionesFuturas = prestamo.Amortizaciones
            .Where(a => a.FechaPago > fechaAbono && !a.Pagado)
            .OrderBy(a => a.FechaPago)
            .ToList();

        foreach (var amortizacion in amortizacionesFuturas)
        {
            amortizacion.MontoInteres = nuevoCapital * (prestamo.InteresMensual / 100m);
            amortizacion.MontoTotal = amortizacion.MontoInteres;
            amortizacion.SaldoPendiente = nuevoCapital;
        }

        await _prestamoRepository.UpdateAsync(prestamo);
    }
}