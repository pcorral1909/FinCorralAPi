using FinCorralApi.Domain.Entities;
using FinCorralApi.Domain.Enums;
using FinCorralApi.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinCorralApi.Infrastructure.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<OtpEntry> OtpEntries => Set<OtpEntry>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Prestamo> Prestamos => Set<Prestamo>();
    public DbSet<Abono> Abonos => Set<Abono>();
    public DbSet<Amortizacion> Amortizaciones => Set<Amortizacion>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasIndex(x => x.Email).IsUnique(false);
            b.HasIndex(x => x.Phone).IsUnique(false);
            b.Property(x => x.Email).HasMaxLength(320);
            b.Property(x => x.Phone).HasMaxLength(24);
            b.Property(x => x.PasswordHash).HasMaxLength(512);
        });

        modelBuilder.Entity<OtpEntry>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasIndex(x => new { x.Destination, x.Purpose });
            b.Property(x => x.OtpHash).HasMaxLength(512);
            b.Property(x => x.Purpose).HasMaxLength(64);
        });

        modelBuilder.Entity<Cliente>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Nombre).HasMaxLength(200).IsRequired();
            b.Property(x => x.Email).HasMaxLength(320);
            b.Property(x => x.Telefono).HasMaxLength(24);

            b.HasMany(x => x.Prestamos)
            .WithOne(x => x.Cliente)
            .HasForeignKey(x => x.ClienteId)
            .OnDelete(DeleteBehavior.Cascade); // 🔥
        });

        modelBuilder.Entity<Prestamo>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasOne(x => x.Cliente)
             .WithMany(x => x.Prestamos)
             .HasForeignKey(x => x.ClienteId);
            b.HasMany(x => x.Abonos)
               .WithOne(x => x.Prestamo)
               .HasForeignKey(x => x.PrestamoId)
               .IsRequired()                           // 🔒
               .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(x => x.Amortizaciones)
                .WithOne(x => x.Prestamo)
                .HasForeignKey(x => x.PrestamoId)
                .IsRequired()                           // 🔒
                .OnDelete(DeleteBehavior.Cascade);

            b.Property(x => x.Monto).HasPrecision(18, 2);
            b.Property(x => x.PagoQuincenal).HasPrecision(18, 2);
            b.Property(x => x.InteresMensual).HasPrecision(5, 2);



            // 🔥 CONVERSIÓN ENUM ↔ STRING NUMÉRICO
            b.Property(x => x.TipoPrestamo)
             .HasConversion(
                 v => ((int)v).ToString(),          // enum → "1"
                 v => (TipoPrestamo)int.Parse(v)    // "1" → enum
             );
        });

        modelBuilder.Entity<Abono>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasOne(x => x.Prestamo)
             .WithMany(x => x.Abonos)
             .HasForeignKey(x => x.PrestamoId);
            b.Property(x => x.Monto).HasPrecision(18, 2);
            b.Property(x => x.Tipo).HasMaxLength(50);
        });

        modelBuilder.Entity<Amortizacion>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasOne(x => x.Prestamo)
             .WithMany(x => x.Amortizaciones)
             .HasForeignKey(x => x.PrestamoId);
            b.Property(x => x.MontoCapital).HasPrecision(18, 2);
            b.Property(x => x.MontoInteres).HasPrecision(18, 2);
            b.Property(x => x.MontoTotal).HasPrecision(18, 2);
            b.Property(x => x.SaldoPendiente).HasPrecision(18, 2);
        });
    }
}