using System.Security.Cryptography;
using System.Text;

namespace Brunozec.Common.Helpers.Cryptography;

public class SHA512Crypto
{
    public static string GenerateHash(string value)
    {
        const string salt = @"salt";

        if (string.IsNullOrEmpty(value))
            return null;

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