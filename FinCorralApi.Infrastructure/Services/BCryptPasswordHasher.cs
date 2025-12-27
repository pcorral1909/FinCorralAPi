using System;
using FinCorralApi.Application.Interfaces;
using BCrypt.Net;

namespace FinCorralApi.Infrastructure.Services;

public sealed class BCryptPasswordHasher : IPasswordHasher
{
    private readonly int _workFactor = 12; // ajustar según CPU/latencia esperada

    public string Hash(string input)
    {
        return BCrypt.Net.BCrypt.HashPassword(input, _workFactor);
    }

    public bool Verify(string input, string hash)
    {
        if (string.IsNullOrEmpty(hash)) return false;
        return BCrypt.Net.BCrypt.Verify(input, hash);
    }
}