using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FinCorralApi.Application.Interfaces;
using FinCorralApi.Infrastructure.Data;
using FinCorralApi.Domain.Entities;

namespace FinCorralApi.Infrastructure.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => _db.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public Task<User?> GetByPhoneOrEmailAsync(string phone, string email, CancellationToken cancellationToken)
        => _db.Users.FirstOrDefaultAsync(u => u.Phone == phone || u.Email == email, cancellationToken);

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        await _db.Users.AddAsync(user, cancellationToken).ConfigureAwait(false);
    }

    public Task UpdateAsync(User user, CancellationToken cancellationToken)
    {
        _db.Users.Update(user);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
        => _db.SaveChangesAsync(cancellationToken);
}