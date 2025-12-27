using MediatR;
using FinCorralApi.Application.DTOs;

namespace FinCorralApi.Application.Queries;

public record GetPrestamosByClienteQuery(int ClienteId) : IRequest<List<PrestamoConAmortizacionDto>>;