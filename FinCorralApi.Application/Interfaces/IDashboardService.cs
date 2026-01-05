using FinCorralApi.Application.DTOs;
using FinCorralApi.Domain.Entities;

namespace FinCorralApi.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardResumenDto> ObtenerResumenAsync();

}