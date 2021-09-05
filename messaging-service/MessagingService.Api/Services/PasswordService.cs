using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Security.Cryptography;

namespace MessagingService.Api.Services
{
    public interface IPasswordService
    {
        /// <summary>
        /// Generates a 128-bit salt using a cryptographically strong random sequence of nonzero values
        /// </summary>
        byte[] GenerateSalt();


        /// <summary>
        /// Derives a 256-bit subkey usin HMACSHA256
        /// </summary>
        /// <param name="plainText">text to hash</param>
        /// <param name="salt">salt to hashing plain text</param>
        string HashText(string plainText, byte[] salt);
    }
    public class PasswordService : IPasswordService
    {
        public byte[] GenerateSalt()
        {
            byte[] salt = new byte[128 / 8];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetNonZeroBytes(salt);
            }
            return salt;
        }

        public string HashText(string plainText, byte[] salt)
        {
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: plainText,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
            return hashed;
        }
    }
}
