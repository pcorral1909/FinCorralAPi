using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using FinCorralApi.Infrastructure.Data;

namespace FinCorralApi.Infrastructure;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer("Server=tcp:prestamos-sqlserver-01.database.windows.net,1433;Initial Catalog=prestamosdb;Persist Security Info=False;User ID=adminsql;Password={your_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

        return new AppDbContext(optionsBuilder.Options);
    }
}