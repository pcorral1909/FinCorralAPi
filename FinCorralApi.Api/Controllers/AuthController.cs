using Microsoft.AspNetCore.Mvc;
using FinCorralApi.Application.DTOs;
using FinCorralApi.Application.Services;
using MediatR;

namespace FinCorralApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : BaseController
{
    private readonly IJwtService _jwtService;

    public AuthController(IJwtService jwtService, IMediator mediator) : base(mediator)
    {
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        // Validar credenciales (implementar según tu lógica)
        if (dto.Email == "admin@fincorral.com" && dto.Password == "Admin123!")
        {
            var token = _jwtService.GenerateToken("1", dto.Email);
            var refreshToken = _jwtService.GenerateRefreshToken();
            
            return Ok(new LoginResponseDto(token, refreshToken, DateTime.UtcNow.AddHours(24)));
        }
        
        return Unauthorized("Credenciales inválidas");
    }
}