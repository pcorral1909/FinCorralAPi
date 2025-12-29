using MediatR;
using FinCorralApi.Application.DTOs;

namespace FinCorralApi.Application.Queries;

public record GetTiposPrestamoQuery() : IRequest<List<TipoPrestamoDto>>;