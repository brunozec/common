using System.Security.Cryptography;
using System.Text;

namespace Brunozec.Common.Helpers.Cryptography;

public sealed class SHA512Crypto
{
    public static string GenerateHash(string salt, string value)
    {
        if (salt == null) throw new ArgumentNullException(nameof(salt));
        if (value == null) throw new ArgumentNullException(nameof(value));

        using var hasher = SHA512.Create();
        var encoding = new UTF8Encoding();

        var array = hasher.ComputeHash(encoding.GetBytes($"{salt}{value}"));

        var result = new StringBuilder();

        foreach (var item in array)
        {
            // Convertendo para Hexadecimal
            result.Append(item.ToString("x2"));
        }

        return result.ToString();
    }
}