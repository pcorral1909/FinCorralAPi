using FinCorralApi.Domain.Entities;

namespace FinCorralApi.Application.Interfaces;

public interface IPrestamoRepository
{
    Task<Prestamo?> GetByIdAsync(int id);
    Task<Prestamo> CreateAsync(Prestamo prestamo);
    Task<List<Prestamo>> GetByClienteIdAsync(int clienteId);
    Task UpdateAsync(Prestamo prestamo);
    Task<bool> DeletePrestamoAsync(int id);   
}