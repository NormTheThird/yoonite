using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System;

namespace Yoonite.Common.Helpers
{
    public static class Security
    {
        public static string GetHash(string input)
        {
            var hashAlgorithm = new SHA256CryptoServiceProvider();
            byte[] byteValue = Encoding.UTF8.GetBytes(input);
            byte[] byteHash = hashAlgorithm.ComputeHash(byteValue);
            return Convert.ToBase64String(byteHash);
        }

        /// <summary>
        /// Salted password hashing with PBKDF2-SHA1.
        /// </summary>
        //private const int SALT_BYTE_SIZE = 24;
        //private const int HASH_BYTE_SIZE = 24;
        //private const int PBKDF2_ITERATIONS = 1000;
        //private const int ITERATION_INDEX = 0;
        //private const int SALT_INDEX = 1;
        //private const int PBKDF2_INDEX = 2;

        /// <summary>
        /// Create a hash of the password to store in the database
        /// </summary>
        /// <param name="password"></param>
        /// <returns>byte[]</returns>
        public static byte[] HashPassword(string password)
        {
            try
            {
                // Throw error if password is empty
                if (string.IsNullOrEmpty(password))
                    throw new ApplicationException("Pass word is null or empty");

                // Generate a random number and of bytes for the salt
                var randomNumberGenerator = new RNGCryptoServiceProvider();
                var salt = new byte[32];
                randomNumberGenerator.GetBytes(salt);

                // Hash the password
                byte[] hash;
                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 29491))
                    hash = pbkdf2.GetBytes(32);

                // Return the hashed password
                return hash.Concat(salt).ToArray();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Validates a password given a hash of the correct one.
        /// </summary>
        /// <param name="password"></param>
        /// <param name="hash"></param>
        /// <returns>True if the password is correct. False otherwise.</returns>
        public static bool ValidatePassword(string password, byte[] hash)
        {
            try
            {
                // Extract the parameters from the hash
                var newHash = hash.Take(hash.Length / 2).ToArray();
                var salt = hash.Skip(hash.Length / 2).ToArray();

                // Hash the password
                byte[] testHash;
                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 29491))
                    testHash = pbkdf2.GetBytes(32);

                // Check Password
                return SlowEquals(newHash, testHash);
            }
            catch (Exception)
            {
                return false;
            }

        }

        #region Private Methods

        /// <summary>
        /// Computes the PBKDF2-SHA1 hash of a password.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <param name="salt">The salt.</param>
        /// <param name="iterations">The PBKDF2 iteration count.</param>
        /// <param name="outputBytes">The length of the hash to generate, in bytes.</param>
        /// <returns>A hash of the password.</returns>
        private static byte[] PBKDF2(string password, byte[] salt, int iterations, int outputBytes)
        {
            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt);
            pbkdf2.IterationCount = iterations;
            return pbkdf2.GetBytes(outputBytes);
        }

        /// <summary>
        /// Compares two byte arrays in length-constant time. This comparison
        /// method is used so that password hashes cannot be extracted from
        /// on-line systems using a timing attack and then attacked off-line.
        /// </summary>
        /// <param name="a">The first byte array.</param>
        /// <param name="b">The second byte array.</param>
        /// <returns>True if both byte arrays are equal. False otherwise.</returns>
        private static bool SlowEquals(byte[] a, byte[] b)
        {
            try
            {
                uint diff = (uint)a.Length ^ (uint)b.Length;
                for (int i = 0; i < a.Length && i < b.Length; i++)
                    diff |= (uint)(a[i] ^ b[i]);
                return diff == 0;
            }
            catch (Exception)
            {
                // Write to error log
                return false;
            }

        }

        #endregion
    }
}