using System.Security.Cryptography;
using System.Text;

namespace Brunozec.Common.Helpers.Cryptography;

public sealed class AesCryptography
{
    public AesCryptography(string key, string iv)
    {
        Key = Encoding.ASCII.GetBytes(key);
        IV = Encoding.ASCII.GetBytes(iv);
    }

    public AesCryptography()
    {
        Key = Encoding.ASCII.GetBytes(ConfigHelper.GetSetting("AES:Key"));
        IV = Encoding.ASCII.GetBytes(ConfigHelper.GetSetting("AES:IV"));
    }

    public byte[] IV { get; set; }

    public byte[] Key { get; set; }

    public string EncryptStringToBytes_Aes(string plainText)
    {
        return Encoding.ASCII.GetString(EncryptStringToBytes_Aes_Bytes(plainText));
    }

    public byte[] EncryptStringToBytes_Aes_Bytes(string plainText)
    {
        // Check arguments.
        if (plainText == null || plainText.Length <= 0)
            throw new ArgumentNullException(nameof(plainText));

        // Create an Aes object
        // with the specified key and iv.
        using Aes aesAlg = Aes.Create();
        aesAlg.Key = Key;
        aesAlg.IV = IV;

        // Create an encryptor to perform the stream transform.
        var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        // Create the streams used for encryption.
        using var msEncrypt = new MemoryStream();
        using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
        using var swEncrypt = new StreamWriter(csEncrypt);
        //Write all data to the stream.
        swEncrypt.Write(plainText);
        var encrypted = msEncrypt.ToArray();

        // Return the encrypted bytes from the memory stream.
        return encrypted;
    }

    public string DecryptStringFromBytes_Aes(string cipherText)
    {
        return DecryptStringFromBytes_Aes(Encoding.ASCII.GetBytes(cipherText));
    }

    public string DecryptStringFromBytes_Aes(byte[] cipherText)
    {
        // Check arguments.
        if (cipherText == null || cipherText.Length <= 0)
            throw new ArgumentNullException(nameof(cipherText));

        // Create an Aes object
        // with the specified key and iv.
        using Aes aesAlg = Aes.Create();
        aesAlg.Key = Key;
        aesAlg.IV = IV;

        // Create a decryptor to perform the stream transform.
        var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

        // Create the streams used for decryption.
        using var msDecrypt = new MemoryStream(cipherText);
        using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        using var srDecrypt = new StreamReader(csDecrypt);
        // Read the decrypted bytes from the decrypting stream
        // and place them in a string.
        return srDecrypt.ReadToEnd();
    }
}