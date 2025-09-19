using System.Security.Cryptography;

namespace Music.Services;

public static class Cryptography
{
    public static byte[] GenerateSaltedHash(byte[] raw, byte[] salt)
    {
        var salted = raw.Concat(salt).ToArray();
        return SHA256.HashData(salted);
    }
}
