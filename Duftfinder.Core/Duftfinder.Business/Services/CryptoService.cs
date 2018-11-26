using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using Duftfinder.Domain.Interfaces.Services;

namespace Duftfinder.Business.Services
{
    /// <summary>
    /// Contains the encryption and decryption for the password.
    /// </summary>
    /// <seealso>adesso SzkB.Ehypo project</seealso>
    public class CryptoService : ICryptoService
    {
        private const int HashByteSize = 20;

        private const int Pbkdf2Iterations = 20000;

        private const int SaltByteSize = 16;

        public string GeneratePasswordHash(string password)
        {
            byte[] salt = new byte[SaltByteSize];

            // Generate a random salt
            using (RNGCryptoServiceProvider csprng = new RNGCryptoServiceProvider())
            {
                csprng.GetBytes(salt);
            }

            // Hash the password and encode the parameters
            byte[] hash = EncodeToHash(password, salt, Pbkdf2Iterations, HashByteSize);
            return Pbkdf2Iterations + ":" + Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
        }

        public bool ValidatePassword(string password, string correctHash)
        {
            // Extract the parameters from the hash
            char[] delimiter = { ':' };
            if (string.IsNullOrEmpty(correctHash) || string.IsNullOrEmpty(password))
            {
                return false;
            }

            string[] split = correctHash.Split(delimiter);
            if (split.Length != 3)
            {
                return false;
            }

            int iterations = int.Parse(split[0], CultureInfo.InvariantCulture);
            byte[] salt = Convert.FromBase64String(split[1]);
            byte[] hash = Convert.FromBase64String(split[2]);
            byte[] testHash = EncodeToHash(password, salt, iterations, hash.Length);

            return ConstantTimeEquals(hash, testHash);
        }

        private static bool ConstantTimeEquals(IList<byte> a, IList<byte> b)
        {
            uint diff = (uint)a.Count ^ (uint)b.Count;

            for (int i = 0; (i < a.Count) && (i < b.Count); i++)
            {
                diff |= (uint)(a[i] ^ b[i]);
            }

            return diff == 0;
        }

        private static byte[] EncodeToHash(string password, byte[] salt, int iterations, int outputBytes)
        {
            using (Rfc2898DeriveBytes rfcEncoder = new Rfc2898DeriveBytes(password, salt))
            {
                rfcEncoder.IterationCount = iterations;
                return rfcEncoder.GetBytes(outputBytes);
            }
        }
    }
}
