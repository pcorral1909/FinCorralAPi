using Microsoft.EntityFrameworkCore;
using FinCorralApi.Application.Interfaces;
using FinCorralApi.Domain.Entities;
using FinCorralApi.Infrastructure.Data;

namespace FinCorralApi.Infrastructure.Repositories;

public class PrestamoRepository : IPrestamoRepository
{
    private readonly AppDbContext _context;

    public PrestamoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Prestamo?> GetByIdAsync(int id)
    {
        return await _context.Prestamos
            .Include(p => p.Cliente)
            .Include(p => p.Amortizaciones)
            .Include(p => p.Abonos)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Prestamo> CreateAsync(Prestamo prestamo)
    {
        _context.Prestamos.Add(prestamo);
        await _context.SaveChangesAsync();
        return prestamo;
    }

    public async Task<List<Prestamo>> GetByClienteIdAsync(int clienteId)
    {
        return await _context.Prestamos
            .Where(p => p.ClienteId == clienteId)
            .Include(p => p.Amortizaciones)
            .ToListAsync();
    }

    public async Task UpdateAsync(Prestamo prestamo)
    {
        _context.Prestamos.Update(prestamo);
        await _context.SaveChangesAsync();
    }
    public async Task<bool> DeletePrestamoAsync(int prestamoId)
    {
        var rows = await _context.Prestamos
            .Where(p => p.Id == prestamoId)
            .ExecuteDeleteAsync();

        return rows > 0;
    }
}