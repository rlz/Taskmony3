using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Taskmony.Services;

public class PasswordHasher : IPasswordHasher, IDisposable
{
    private readonly RandomNumberGenerator _rng;
    private const KeyDerivationPrf Prf = KeyDerivationPrf.HMACSHA512;
    private const int IterCount = 10000;
    private const int NumOfBytesRequested = 256 / 8;
    private const int SaltSize = 128 / 8;

    public PasswordHasher()
    {
        _rng = RandomNumberGenerator.Create();
    }

    public string HashPassword(string password)
    {
        if (password is null)
        {
            throw new ArgumentNullException(nameof(password));
        }

        var salt = new byte[SaltSize];
        _rng.GetBytes(salt);

        var subkey = KeyDerivation.Pbkdf2(password, salt, Prf, IterCount, NumOfBytesRequested);

        // Add the salt in front of the hash
        var hashBytes = new byte[salt.Length + subkey.Length];

        Buffer.BlockCopy(salt, 0, hashBytes, 0, salt.Length);
        Buffer.BlockCopy(subkey, 0, hashBytes, salt.Length, subkey.Length);

        return Convert.ToBase64String(hashBytes);
    }

    public bool VerifyPassword(string password, string hash)
    {
        if (password is null)
        {
            throw new ArgumentNullException(nameof(password));
        }

        if (hash is null)
        {
            throw new ArgumentNullException(nameof(hash));
        }

        var decodedHash = Convert.FromBase64String(hash);

        return decodedHash.Length != 0 && VerifyPassword(password, decodedHash);
    }

    private bool VerifyPassword(string password, byte[] hash)
    {
        try
        {
            byte[] salt = new byte[SaltSize];

            Buffer.BlockCopy(hash, 0, salt, 0, salt.Length);

            int subKeyLength = hash.Length - salt.Length;

            if (subKeyLength < SaltSize)
            {
                return false;
            }

            var expectedSubkey = new byte[subKeyLength];

            Buffer.BlockCopy(hash, salt.Length, expectedSubkey, 0, expectedSubkey.Length);

            var actualSubkey = KeyDerivation.Pbkdf2(password, salt, Prf, IterCount, subKeyLength);

            return ByteArraysEqual(actualSubkey, expectedSubkey);
        }
        catch
        {
            return false;
        }
    }

    private static bool ByteArraysEqual(byte[] a, byte[] b)
    {
        if (a == null && b == null)
        {
            return true;
        }

        if (a == null || b == null || a.Length != b.Length)
        {
            return false;
        }

        bool areSame = true;

        for (int i = 0; i < a.Length; i++)
        {
            areSame &= a[i] == b[i];
        }

        return areSame;
    }

    public void Dispose() => _rng.Dispose();
}