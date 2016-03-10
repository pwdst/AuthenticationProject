namespace AuthenticationProject.Data.Extensions.Security
{
    using System.Security.Cryptography;

    internal static class UserExtensions
    {
        internal static byte[] GenerateSalt(int saltLength = 32)
        {
            using (var randomNumberGenerator = RandomNumberGenerator.Create())
            {
                var randomNumber = new byte[saltLength];
                randomNumberGenerator.GetBytes(randomNumber);

                return randomNumber;
            }
        }

        internal static bool PasswordHashesEqual(byte[] providedHash, byte[] savedHash)
        {
            if (providedHash == null && savedHash == null)
            {
                return true;
            }
            if (providedHash == null || savedHash == null || providedHash.Length != savedHash.Length)
            {
                return false;
            }

            var bytesMatch = true;

            for (var i = 0; i < providedHash.Length; i++)
            {
                bytesMatch &= (providedHash[i] == savedHash[i]);
            }

            return bytesMatch;
        }

        internal static byte[] HashPassword(byte[] passwordBytes, byte[] salt, int numberOfRounds)
        {
            using (var rfc2898 = new Rfc2898DeriveBytes(passwordBytes, salt, numberOfRounds))
            {
                return rfc2898.GetBytes(32);
            }
        }
    }
}