using System.Security.Cryptography;

namespace Password11.src.Util;

public static class Encryption
{
    public const bool EncryptionDisabled = false;

    public static readonly byte[] MagicTest =
        { 121, 111, 117, 116, 117, 46, 98, 101, 47, 100, 81, 119, 52, 119, 57, 87, 103, 88, 99, 81 };

    public static byte[] EncryptAes(this byte[] data, string key)
    {
        if (EncryptionDisabled)
            return data;
        using var aes = Aes.Create();
        aes.Key = Sha256(key.EncodeUtf8());
        aes.IV = new byte[16];
        Random.Shared.NextBytes(aes.IV);
        var tempdata = data.ToList();
        tempdata.InsertRange(0, MagicTest);
        data = tempdata.ToArray();
        using var stream = new MemoryStream();
        var encryptor = aes.CreateEncryptor();
        using (var encryptStream = new CryptoStream(stream, encryptor, CryptoStreamMode.Write))
        {
            encryptStream.Write(data, 0, data.Length);
        }

        var encrypted = stream.ToArray();
        var output = new List<byte>();
        output.AddRange(aes.IV);
        output.AddRange(encrypted);
        return output.ToArray();
    }

    public static byte[] DecryptAes(this byte[] data, string key)
    {
        try
        {
            if (EncryptionDisabled)
                return data;
            using var aes = Aes.Create();
            aes.Key = Sha256(key.EncodeUtf8());
            aes.IV = data.ToList().GetRange(0, 16).ToArray();
            data = data.ToList().GetRange(16, data.Length - 16).ToArray();

            using var stream = new MemoryStream(data);
            var encryptor = aes.CreateDecryptor();
            using var decryptStream = new CryptoStream(stream, encryptor, CryptoStreamMode.Read);
            using var resultStream = new MemoryStream();
            decryptStream.CopyTo(resultStream);
            var decrypted = resultStream.ToArray();
            var magicCheck = decrypted.ToList().GetRange(0, MagicTest.Length).ToArray();
            if (!MagicTest.SequenceEqual(magicCheck)) throw new CryptographicException("Wrong key");

            return decrypted.ToList().GetRange(MagicTest.Length, decrypted.Length - MagicTest.Length).ToArray();
        }
        catch
        {
            throw new CryptographicException("Data corrupted or wrong password is entered");
        }
    }

    public static byte[] Sha256(this byte[] bytes)
    {
        using var hash = SHA256.Create();
        return hash.ComputeHash(bytes);
    }
}