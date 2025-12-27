using FinCorralApi.Domain.Entities;

namespace FinCorralApi.Application.Interfaces;

public interface IClienteRepository
{
    Task<Cliente?> GetByIdAsync(int id);
    Task<Cliente> CreateAsync(Cliente cliente);
    Task<List<Cliente>> GetAllAsync();
}