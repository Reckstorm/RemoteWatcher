using System.Security.Cryptography;
using System.Text;

namespace API.Services;

public static class EncryptorService
{
    private static byte[] Crypt(string data, string salt = "")
    {
        SHA512 crypt = SHA512.Create();
        byte[] bytes = Encoding.UTF8.GetBytes(salt + data + salt);
        byte[] bytesHash = crypt.ComputeHash(bytes);
        crypt.Clear();
        return bytesHash;
    }

    private static string BuildString(byte[] bytesHash)
    {
        StringBuilder builder = new StringBuilder();
        foreach (var item in bytesHash)
        {
            builder.Append(item.ToString("x2"));
        }
        return builder.ToString();
    }
    public static async Task<string> SimpleEnc(string data) => await Task.Run(() => BuildString(Crypt(data)));

    public static async Task<string> SimpleSaltEnc(string data, string salt = "") => await Task.Run(() => BuildString(Crypt(data, salt)));

    public static async Task<string> MultiEnc(string data, int count = 1)
    {
        string newData = data;
        for (int i = 0; i < count; i++)
        {
            newData = await SimpleEnc(newData);
        }
        return newData;
    }
    public static async Task<string> MultiSaltEnc(string data, string salt = "", int count = 1)
    {
        string newData = data;
        for (int i = 0; i < count; i++)
        {
            newData = await SimpleSaltEnc(newData, salt);
        }
        return newData;
    }
}