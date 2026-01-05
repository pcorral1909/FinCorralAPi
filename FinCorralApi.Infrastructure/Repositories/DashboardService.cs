using FinCorralApi.Application.DTOs;
using FinCorralApi.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinCorralApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinCorralApi.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _context;

        public DashboardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardResumenDto> ObtenerResumenAsync()
        {
            // 1️⃣ Total prestado (histórico)
            var totalPrestado = await _context.Prestamos
                .SumAsync(p => p.Monto);

            // 2️⃣ Saldo pendiente (última amortización por préstamo)
            var saldoPendiente = await _context.Amortizaciones
                .GroupBy(a => a.PrestamoId)
                .Select(g => g
                    .OrderByDescending(x => x.Id) // o NumeroPago si lo tienes
                    .First().SaldoPendiente)
                .SumAsync();

            // 3️⃣ Intereses ganados (solo amortizaciones pagadas)
            var interesesGanados = await _context.Amortizaciones
                .Where(a => a.Pagado) // ⚠️ asumo que tienes este campo
                .SumAsync(a => a.MontoInteres);

            // 4️⃣ Préstamos activos (saldo pendiente > 0)
            var prestamosActivos = await _context.Amortizaciones
                .GroupBy(a => a.PrestamoId)
                .CountAsync(g =>
                    g.OrderByDescending(x => x.Id)
                     .First().SaldoPendiente > 0);

            // 5️⃣ Total clientes
            var totalClientes = await _context.Clientes.CountAsync();

            return new DashboardResumenDto
            {
                TotalPrestado = totalPrestado,
                SaldoPendiente = saldoPendiente,
                InteresesGanados = interesesGanados,
                PrestamosActivos = prestamosActivos,
                TotalClientes = totalClientes
            };
        }
    }

}
