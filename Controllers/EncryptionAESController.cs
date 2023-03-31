using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace EncryptionAesAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EncryptionController : ControllerBase
    {
        private readonly ILogger<EncryptionController> _logger;

        public EncryptionController(ILogger<EncryptionController> logger)
        {
            _logger = logger;
        }

        [HttpPost("encrypt")]
        public IActionResult Encrypt([FromBody]EncryptionRequest request)
        {
            string encryptedText = EncryptionLibrary.Encryption.Encrypt(request.Text, request.Key);
            return Ok(new EncryptionResponse { Text = encryptedText });
        }

        [HttpPost("decrypt")]
        public IActionResult Decrypt([FromBody]EncryptionRequest request)
        {
            string decryptedText = EncryptionLibrary.Encryption.Decrypt(request.Text, request.Key);
            return Ok(new EncryptionResponse { Text = decryptedText });
        }
    }

    public class EncryptionRequest
    {
        public string? Text { get; set; }
        public string? Key { get; set; }
    }

    public class EncryptionResponse
    {
        public string? Text { get; set; }
    }
}

namespace EncryptionLibrary
{
    public static class Encryption
    {
        public static string Encrypt(string plainText, string key)
        {
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);

            using (var aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.IV = new byte[aes.BlockSize / 8];
                aes.Mode = CipherMode.CBC;

                using (var encryptor = aes.CreateEncryptor())
                {
                    byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                    return Convert.ToBase64String(encryptedBytes);
                }
            }
        }

        public static string Decrypt(string encryptedText, string key)
        {   
            System.Diagnostics.Debug.WriteLine("information gotten from the ClientSide");
            System.Diagnostics.Debug.WriteLine(key);
            System.Diagnostics.Debug.WriteLine(encryptedText);
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);

            using (var aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.IV = new byte[aes.BlockSize / 8];
                aes.Mode = CipherMode.CBC;

                using (var decryptor = aes.CreateDecryptor())
                {
                    byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                    Console.WriteLine(key);
                    Console.WriteLine(encryptedText);
                    
                    return Encoding.UTF8.GetString(decryptedBytes);
                }
            }
        }
    }
}
