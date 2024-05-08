using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace WinUi3Test.src.Util;

public class Encryption
{
    public static readonly byte[] MagicTest = {121,111,117,116,117,46,98,101,47,100,81,119,52,119,57,87,103,88,99,81};

    public static byte[] Encrypt(string text, string key)
    {
        var data = Encoding.UTF8.GetBytes(text);
        using var aes = Aes.Create();
        aes.Key = Sha256(Encoding.UTF8.GetBytes(key));
        aes.IV = new byte[16];
        Random.Shared.NextBytes(aes.IV);
        var tempdata = data.ToList();
        tempdata.InsertRange(0,MagicTest);
        data = tempdata.ToArray();
        using var stream = new MemoryStream();
        ICryptoTransform encryptor = aes.CreateEncryptor();
        using (var encryptStream = new CryptoStream(stream,encryptor,CryptoStreamMode.Write))
        {
            encryptStream.Write(data,0,data.Length);
        }
        var encrypted = stream.ToArray();
        var output = new List<byte>();
        output.AddRange(aes.IV);
        output.AddRange(encrypted);
        return output.ToArray();
    }

    public static string Decrypt(byte[] data, string key)
    {
        using var aes = Aes.Create();
        aes.Key = Sha256(Encoding.UTF8.GetBytes(key));
        aes.IV = data.ToList().GetRange(0, 16).ToArray();
        data = data.ToList().GetRange(16, data.Length - 16).ToArray();
            
        using var stream = new MemoryStream(data);
        ICryptoTransform encryptor = aes.CreateDecryptor();
        using var decryptStream = new CryptoStream(stream,encryptor,CryptoStreamMode.Read);
        using var resultStream = new MemoryStream();
        decryptStream.CopyTo(resultStream);
        var decrypted = resultStream.ToArray();
        var magicCheck = decrypted.ToList().GetRange(0, MagicTest.Length).ToArray();
        if (!MagicTest.SequenceEqual(magicCheck))
        {
            throw new CryptographicException("Wrong key");
        }
        return Encoding.UTF8.GetString(decrypted.ToList().GetRange(MagicTest.Length,decrypted.Length-MagicTest.Length).ToArray());
    }
    public static byte[] Sha256(byte[] bytes)
    {
        using SHA256 hash = SHA256.Create();
        return hash.ComputeHash(bytes);
    } 
}