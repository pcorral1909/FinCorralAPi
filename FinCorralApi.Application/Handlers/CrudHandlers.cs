using MediatR;
using FinCorralApi.Application.Commands;
using FinCorralApi.Application.DTOs;
using FinCorralApi.Application.Interfaces;
using FinCorralApi.Domain.Entities;

namespace FinCorralApi.Application.Handlers;

public class EliminarClienteHandler : IRequestHandler<EliminarClienteCommand, string>
{
    private readonly IClienteRepository _clienteRepository;

    public EliminarClienteHandler(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<string> Handle(EliminarClienteCommand request, CancellationToken cancellationToken)
    {
        var cliente = await _clienteRepository.DeleteClienteAsync(request.Id);
        if (cliente == null) throw new ArgumentException("Cliente no encontrado");
        
        // Aquí implementarías la lógica de eliminación
        return "Cliente eliminado exitosamente";
    }
}

public class EliminarPrestamoHandler : IRequestHandler<EliminarPrestamoCommand, string>
{
    private readonly IPrestamoRepository _prestamoRepository;

    public EliminarPrestamoHandler(IPrestamoRepository prestamoRepository)
    {
        _prestamoRepository = prestamoRepository;
    }

    public async Task<string> Handle(EliminarPrestamoCommand request, CancellationToken cancellationToken)
    {
        var prestamo = await _prestamoRepository.DeletePrestamoAsync(request.Id);
        if (prestamo == null) throw new ArgumentException("Préstamo no encontrado");
        
        // Aquí implementarías la lógica de eliminación
        return "Préstamo eliminado exitosamente";
    }
}

public class LiquidarPrestamoHandler : IRequestHandler<LiquidarPrestamoCommand, string>
{
    private readonly IPrestamoRepository _prestamoRepository;

    public LiquidarPrestamoHandler(IPrestamoRepository prestamoRepository)
    {
        _prestamoRepository = prestamoRepository;
    }

    public async Task<string> Handle(LiquidarPrestamoCommand request, CancellationToken cancellationToken)
    {
        var prestamo = await _prestamoRepository.GetByIdAsync(request.Id);
        if (prestamo == null) throw new ArgumentException("Préstamo no encontrado");
        
        // Marcar todas las amortizaciones como pagadas
        foreach (var amortizacion in prestamo.Amortizaciones)
        {
            amortizacion.Pagado = true;
        }
        
        await _prestamoRepository.UpdateAsync(prestamo);
        return "Préstamo liquidado exitosamente";
    }
}

public class CrearUsuarioStep1Handler : IRequestHandler<CrearUsuarioStep1Command, int>
{
    private readonly IUsuarioRepository _usuarioRepository;

    public CrearUsuarioStep1Handler(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<int> Handle(CrearUsuarioStep1Command request, CancellationToken cancellationToken)
    {
        var usuario = new Usuario
        {
            Telefono = request.Telefono,
            Email = request.Email,
            Completado = false,
            FechaCreacion = DateTime.UtcNow
        };

        var usuarioCreado = await _usuarioRepository.CreateAsync(usuario);
        return usuarioCreado.Id;
    }
}

public class CrearUsuarioStep2Handler : IRequestHandler<CrearUsuarioStep2Command, UsuarioDto>
{
    private readonly IUsuarioRepository _usuarioRepository;

    public CrearUsuarioStep2Handler(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<UsuarioDto> Handle(CrearUsuarioStep2Command request, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(request.UsuarioId);
        if (usuario == null) throw new ArgumentException("Usuario no encontrado");

        usuario.NombreCompleto = request.NombreCompleto;
        usuario.FechaNacimiento = request.FechaNacimiento;
        usuario.Curp = request.Curp;
        usuario.Completado = true;

        await _usuarioRepository.UpdateAsync(usuario);

        return new UsuarioDto(
            usuario.Id,
            usuario.Telefono,
            usuario.Email,
            usuario.NombreCompleto,
            usuario.FechaNacimiento,
            usuario.Curp,
            usuario.Completado,
            usuario.TienePassword
        );
    }
}

public class CrearPasswordHandler : IRequestHandler<CrearPasswordCommand, string>
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher _passwordHasher;

    public CrearPasswordHandler(IUsuarioRepository usuarioRepository, IPasswordHasher passwordHasher)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<string> Handle(CrearPasswordCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(request.UsuarioId);
        if (usuario == null) throw new ArgumentException("Usuario no encontrado");

        usuario.PasswordHash = _passwordHasher.Hash(request.Password);
        usuario.TienePassword = true;

        await _usuarioRepository.UpdateAsync(usuario);
        return "Contraseña creada exitosamente";
    }
}