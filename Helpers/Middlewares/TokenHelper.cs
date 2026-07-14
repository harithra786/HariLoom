using System.Text.Json;
using System.Text;
using System.Security.Cryptography;
using hariloom.Models.DTOs;

namespace hariloom.Helpers.Middlewares
{
    public static class TokenHelper
    {
        private static readonly string _secretKey = "A9$kL2@xP7#qR4vZ8!mT5cW1&YhN6sD3F0uEJgBzQpXrVtCwUoI";
        public static string EncryptToken(AuthTokenPayload payload)
        {
            var json = JsonSerializer.Serialize(payload);
            var plainBytes = Encoding.UTF8.GetBytes(json);

            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(_secretKey.PadRight(32).Substring(0, 32));
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor();
            var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            var tokenData = aes.IV.Concat(cipherBytes).ToArray();
            return Convert.ToBase64String(tokenData);
        }

        public static AuthTokenPayload DecryptToken(string encryptedToken)
        {
            var tokenData = Convert.FromBase64String(encryptedToken);

            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(_secretKey.PadRight(32).Substring(0, 32));
            aes.IV = tokenData.Take(16).ToArray();

            var cipherBytes = tokenData.Skip(16).ToArray();

            using var decryptor = aes.CreateDecryptor();
            var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
            var json = Encoding.UTF8.GetString(plainBytes);

            return JsonSerializer.Deserialize<AuthTokenPayload>(json);
        }
    }

}
